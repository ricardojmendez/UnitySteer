//#define TRACEDETECTED
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnitySteer;
using UnitySteer.Helpers;
using TickedPriorityQueue;


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
    
    static Dictionary<Collider, DetectableObject> _cachedDetectableObjects = new Dictionary<Collider, DetectableObject>();
	
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
 
	[SerializeField]
	float _detectionRadius = 5;
	
	[SerializeField]
	bool _detectDisabledVehicles;
	
	[SerializeField]
	LayerMask _layersChecked;
	
	[SerializeField]
	bool _drawGizmos = false;

	/// <summary>
	/// How often is the radar updated
	/// </summary>
	[SerializeField]
	float _tickLength = 0.5f;
    
    [SerializeField]
    int _detectLimit = 30;
	
	
	
	Collider[] _detectedColliders;
    DetectableObject[] _detectedObjects;
    Vehicle[] _vehicles;
    DetectableObject[] _obstacles;
	IList<DetectableObject> _ignoredObjects = new List<DetectableObject>();
	
	
	Vehicle _vehicle;
	#endregion
	
    
    public int VehicleCount { get; private set; }
    public int ObstacleCount { get; private set; }
    public int DetectedCount { get; private set; }
    
	
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
			return this._detectionRadius;
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
			return this._detectDisabledVehicles;
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
			return this._drawGizmos;
		}
		set {
			_drawGizmos = value;
		}
	}

	/// <summary>
	/// List of obstacles detected by the radar
	/// </summary>
	public IEnumerable<DetectableObject> Obstacles {
		get { return _obstacles.Where(x => x != null); }

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
	
	public SteeringEventHandler<Radar> OnDetected { get; set; }

	/// <summary>
	/// Gets the vehicle this radar is attached to
	/// </summary>
	public Vehicle Vehicle {
		get {
			return _vehicle;
		}
	}

	/// <summary>
	/// List of vehicles detected among the colliders
	/// </summary>
	public IEnumerable<Vehicle> Vehicles 
	{
		get { return _vehicles.Where(x => x != null); }
	}

	/// <summary>
	/// Layer mask for the object layers checked
	/// </summary>
	public LayerMask LayersChecked {
		get {
			return this._layersChecked;
		}
		set {
			_layersChecked = value;
		}
	}
	#endregion
	
	#region Methods
	protected virtual void Awake() 
	{
		_vehicle = GetComponent<Vehicle>();	
		_transform = transform;
		Ignore(_vehicle); // All radars ignore their own vehicle
        _vehicles = new Vehicle[_detectLimit];
        _obstacles = new DetectableObject[_detectLimit];
        _detectedObjects = new DetectableObject[_detectLimit];
        VehicleCount = 0;
        ObstacleCount = 0;
	}
	
	
    void OnLevelWasLoaded(int level) {
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
	
	
	
	public void OnUpdateRadar(object obj)
	{
		_detectedColliders = Detect();
		FilterDetected();
		if (OnDetected != null)
		{
			OnDetected(new SteeringEvent<Radar>(null, "detect", this));
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
        
        VehicleCount = 0;
        ObstacleCount = 0;
        DetectedCount = 0;

        for (int i = 0; i < _detectLimit; i++) {
            _vehicles[i] = null;
            _obstacles[i] = null;
            _detectedObjects[i] = null;
        }
        
        foreach(var x in _detectedColliders) {
            if (!_cachedDetectableObjects.ContainsKey(x)) {
                _cachedDetectableObjects[x] = x.transform.GetComponent<DetectableObject>();
            }
            var detectable = _cachedDetectableObjects[x];
            if (detectable != null && !_ignoredObjects.Contains(detectable) && DetectedCount < _detectLimit) {
                _detectedObjects[DetectedCount++] = detectable;
            }
        }
        
        for (int i = 0; i < DetectedCount && VehicleCount < _detectLimit; i++) {
            var v = _detectedObjects[i] as Vehicle;
            if (v != null && (v.enabled || _detectDisabledVehicles)) {
                _vehicles[VehicleCount++] = v;
            }
        }
        
        for (int i = 0; i < DetectedCount && ObstacleCount < _detectLimit; i++) {
            var d = _detectedObjects[i] as DetectableObject;
            if (d != null && !(d is Vehicle)) {
                _obstacles[ObstacleCount++] = d;
            }
        }
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
