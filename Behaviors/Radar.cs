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
	
	Transform _transform;
	TickedObject _tickedObject;
	UnityTickedQueue _steeringQueue;
	
	
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
	
	
	
	IEnumerable<Collider> _detected;
	IEnumerable<Vehicle> _vehicles = new List<Vehicle>();
	IEnumerable<DetectableObject> _obstacles = new List<DetectableObject>();
	IList<DetectableObject> _ignoredObjects = new List<DetectableObject>();
	
	
	Vehicle _vehicle;
	#endregion
	
	
	#region Public properties
	/// <summary>
	/// List of currently detected neighbors
	/// </summary>
	public IEnumerable<Collider> Detected 
	{
		get { return _detected; }
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
		get { return _vehicles; }
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
	}
	
	
	void OnEnable()
	{
		_tickedObject = new TickedObject(OnUpdateRadar);
		_tickedObject.TickLength = _tickLength;
		_steeringQueue = UnityTickedQueue.GetInstance("Radar");
		_steeringQueue.Add(_tickedObject);
		_steeringQueue.MaxProcessedPerUpdate = 50;
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
		_detected = Detect();
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
			Debug.Log(sb.ToString());
		}
#endif
	}
		
	
	protected virtual IEnumerable<Collider> Detect()
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
		 * We materialize the list so that we don't select it twice.
		 */
		Profiler.BeginSample("Base FilterDetected");
		var notIgnored =  _detected.Select( d => d.transform.GetComponent<DetectableObject>() ).Except(_ignoredObjects).ToList();
		_vehicles = notIgnored.OfType<Vehicle>().Where( v => v != null && (v.enabled || _detectDisabledVehicles));
		_obstacles = notIgnored.Where( d => d != null && !(d is Vehicle) );
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
		}
	}	
	#endregion
}
