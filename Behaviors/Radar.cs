//#define TRACEDETECTED
using System.Collections.Generic;
using UnityEngine;
using TickedPriorityQueue;

namespace UnitySteer.Base
{

/// <summary>
/// Base class for radars
/// </summary>
/// <remarks>
/// The base Radar class will "ping" an area using Physics.OverlapSphere, but
/// different radars can implement their own detection styles (if for instance
/// they wish to handle a proximity quadtre/octree themselves).
/// 
/// It expects that every object to be added to the radar will have a 
/// DetectableObject on its root.
/// </remarks>
[AddComponentMenu("UnitySteer/Radar/Radar")]
public class Radar: MonoBehaviour {
	#region Private properties
    
	static IDictionary<int, DetectableObject> _cachedDetectableObjects = new SortedDictionary<int, DetectableObject>();
	
	Transform _transform;
	TickedObject _tickedObject;
	UnityTickedQueue _steeringQueue;
	
    [SerializeField]
    string _queueName = "Radar";
    
    /// <summary>
    /// The maximum number of radar update calls processed on the queue per update
    /// </summary>
    /// <remarks>
    /// Notice that this is a limit shared across queue items of the same name, at
    /// least until we have some queue settings, so whatever value is set last for 
    /// the queue will win.  Make sure your settings are consistent for objects of
    /// the same queue.
    /// </remarks>
    [SerializeField]
    int _maxQueueProcessedPerUpdate = 20;
    
    /// <summary>
    /// How often is the radar updated
    /// </summary>
    [SerializeField]
    float _tickLength = 0.5f;
 
	[SerializeField]
	float _detectionRadius = 5;
	
	[SerializeField]
	bool _detectDisabledVehicles;
	
	[SerializeField]
	LayerMask _layersChecked;
	
	[SerializeField]
	bool _drawGizmos = false;

	[SerializeField]
    int _preAllocateSize = 30;
	
	
	
	Collider[] _detectedColliders;
    List<DetectableObject> _detectedObjects;
    List<Vehicle> _vehicles;
    List<DetectableObject> _obstacles;
	IList<DetectableObject> _ignoredObjects = new List<DetectableObject>(10);
	
	#endregion
	
    
	
	#region Public properties
	/// <summary>
	/// List of currently detected neighbors
	/// </summary>
	public IEnumerable<Collider> Detected 
	{
		get { return _detectedColliders; }
	}
	
	/// <summary>
	/// Radar ping detection radius
	/// </summary>
	public float DetectionRadius {
		get {
			return _detectionRadius;
		}
		set {
			_detectionRadius = value;
		}
	}
	
	
	/// <summary>
	/// Indicates if the radar will detect disabled vehicles. 
	/// </summary>
	public bool DetectDisabledVehicles 
	{
		get {
			return _detectDisabledVehicles;
		}
		set {
			_detectDisabledVehicles = value;
		}
	}
	
	/// <summary>
	/// Should the radar draw its gizmos?
	/// </summary>
	public bool DrawGizmos {
		get {
			return _drawGizmos;
		}
		set {
			_drawGizmos = value;
		}
	}

	/// <summary>
	/// List of obstacles detected by the radar
	/// </summary>
	public List<DetectableObject> Obstacles {
		get { return _obstacles; }

	}
	
	/// <summary>
	/// Returns the radars position
	/// </summary>
	public Vector3 Position
	{
		get
		{
			return (Vehicle != null) ? Vehicle.Position : _transform.position;
		}
	}
	
	public System.Action<Radar> OnDetected = delegate{};

	/// <summary>
	/// Gets the vehicle this radar is attached to
	/// </summary>
	public Vehicle Vehicle { get; private set; }

	/// <summary>
	/// List of vehicles detected among the colliders
	/// </summary>
	public List<Vehicle> Vehicles 
	{
		get { return _vehicles; }
	}

	/// <summary>
	/// Layer mask for the object layers checked
	/// </summary>
	public LayerMask LayersChecked {
		get {
			return _layersChecked;
		}
		set {
			_layersChecked = value;
		}
	}
	#endregion
	
	#region Methods
	protected virtual void Awake() 
	{
		Vehicle = GetComponent<Vehicle>();	
		_transform = transform;
        _vehicles = new List<Vehicle>(_preAllocateSize);
        _obstacles = new List<DetectableObject>(_preAllocateSize);
        _detectedObjects = new List<DetectableObject>(_preAllocateSize * 3);
	}
	
	
    void OnLevelWasLoaded(int level) 
	{
        _cachedDetectableObjects.Clear();
    }
    
	void OnEnable()
	{
		_tickedObject = new TickedObject(OnUpdateRadar);
		_tickedObject.TickLength = _tickLength;
		_steeringQueue = UnityTickedQueue.GetInstance(_queueName);
		_steeringQueue.Add(_tickedObject);
		_steeringQueue.MaxProcessedPerUpdate = _maxQueueProcessedPerUpdate;
	}

	
	void OnDisable()
	{
		if (_steeringQueue != null)
		{
			_steeringQueue.Remove(_tickedObject);
		}
	}
	
	
	
	void OnUpdateRadar(object obj)
	{
        Profiler.BeginSample("OnUpdateRadar");
		_detectedColliders = Detect();
		FilterDetected();
		if (OnDetected != null)
		{
			Profiler.BeginSample("Detection event handler");
			OnDetected(this);
			Profiler.EndSample();
		}
#if TRACEDETECTED
		if (DrawGizmos)
		{
			Debug.Log(gameObject.name+" detected at "+Time.time);
			var sb = new System.Text.StringBuilder(); 
			foreach (var v in Vehicles)
			{
				sb.Append(v.gameObject.name);
				sb.Append(" ");
				sb.Append(v.Position);
				sb.Append(" ");
			}
			foreach (var o in Obstacles)
			{
				sb.Append(o.gameObject.name);
				sb.Append(" ");
				sb.Append(o.Position);
				sb.Append(" ");
			}
			Debug.Log(sb.ToString());
		}
#endif
        Profiler.EndSample();
	}

	public void UpdateRadar()
	{
		OnUpdateRadar(null);
	}
		
	
	protected virtual Collider[] Detect()
	{
		return Physics.OverlapSphere(Position, DetectionRadius, LayersChecked);
	}
	
	protected virtual void FilterDetected()
	{
		/*
		 * For each detected item, obtain the DetectableObject it has.
		 * We could have allowed people to have multiple DetectableObjects 
		 * on a transform hierarchy, but this ends up with us having to do
		 * calls to GetComponentsInChildren, which gets really expensive.
		 * 
		 * I *do not* recommend changing this to GetComponentsInChildren.
		 * As a reference, whenever the radar fired up near a complex object
		 * (say, a character model) obtaining the list of DetectableObjects
		 * took about 75% of the time used for the frame.
         * 
		 */
		Profiler.BeginSample("Base FilterDetected");

        _vehicles.Clear();
        _obstacles.Clear();
        _detectedObjects.Clear();
        

		Profiler.BeginSample("Initial detection");
		for (int i = 0; i < _detectedColliders.Length; i++)
		{
			var x = _detectedColliders[i].GetInstanceID();
            if (!_cachedDetectableObjects.ContainsKey(x)) 
			{
                _cachedDetectableObjects[x] = _detectedColliders[i].GetComponent<DetectableObject>();
            }
            var detectable = _cachedDetectableObjects[x];
            // It's possible that d != null but that d.Equals(null) if the
            // game object has been marked as destroyed by Unity between
            // detection and filtering.
            if (detectable != null && 
			    detectable != Vehicle &&
			    !detectable.Equals(null) && 
			    !_ignoredObjects.Contains(detectable)) 
			{
                _detectedObjects.Add(detectable);
            }
        }
		Profiler.EndSample();
        
		Profiler.BeginSample("Filtering out vehicles");
        for (int i = 0; i < _detectedObjects.Count; i++) 
		{
            var d = _detectedObjects[i];
            var v = d as Vehicle;
            if (v != null && (v.enabled || _detectDisabledVehicles)) {
                _vehicles.Add(v);
            }
            else {
                _obstacles.Add(d);
            }
        }
		Profiler.EndSample();
		Profiler.EndSample();
	}
	
	/// <summary>
	/// Tells the radar to ignore the detectable object when filtering the vehicles or objects detected
	/// </summary>
	/// <param name="o">
	/// An object to be ignored<see cref="DetectableObject"/>
	/// </param>
	/// <returns>The radar</returns>
	public Radar Ignore(DetectableObject o)
	{
		if (o != null)
		{
			_ignoredObjects.Add(o);
		}
		return this;
	}
	
	/// <summary>
	/// Tells the radar to no longer ignore the detectable object when filtering the vehicles or objects detected
	/// </summary>
	/// <param name="o">
	/// An object to remove from the ignore list<see cref="DetectableObject"/>
	/// </param>
	/// <returns>The radar</returns>
	public Radar DontIgnore(DetectableObject o)
	{
		if (_ignoredObjects.Contains(o))
		{
			_ignoredObjects.Remove(o);
		}
		return this;
	}
	#endregion
	
	#region Unity methods
	void OnDrawGizmos()
	{
		if (_drawGizmos)
		{
			var pos = (Vehicle == null) ? transform.position : Vehicle.Position;
			
			Gizmos.color = Color.cyan;
			Gizmos.DrawWireSphere(pos, DetectionRadius);
#if TRACEDETECTED
			if (Application.isPlaying) {
				foreach (var v in Vehicles)
					Gizmos.DrawLine(pos, v.gameObject.transform.position);
				foreach (var o in Obstacles)
					Gizmos.DrawLine(pos, o.gameObject.transform.position);
			}
#endif
		}
	}	
	#endregion
}

}
