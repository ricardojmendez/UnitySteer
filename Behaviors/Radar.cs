using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnitySteer;
using UnitySteer.Helpers;


/// <summary>
/// Base class for radars
/// </summary>
/// <remarks>
/// The base Radar class will "ping" an area using Physics.OverlapSphere, but
/// different radars can implement their own detection styles (if for instance
/// they wish to handle a proximity quadtre/octree themselves).
/// </remarks>
public class Radar: MonoBehaviour {
	#region Private properties
	
	[SerializeField]
	float _detectionRadius = 5;
	
	[SerializeField]
	bool _detectDisabledVehicles;
	
	[SerializeField]
	LayerMask _layersChecked;
	
	[SerializeField]
	bool _drawGizmos = false;
	
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
	/// List of obstacles detected by the radar
	/// </summary>
	public IEnumerable<DetectableObject> Obstacles {
		get { return _obstacles; }

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
	protected virtual void Awake() {
		_vehicle = GetComponent<Vehicle>();	
		Ignore(_vehicle); // All radars ignore their own vehicle
	}
	
	public void Update()
	{
		_detected = Detect();
		FilterDetected();
		if (OnDetected != null)
		{
			OnDetected(new SteeringEvent<Radar>(null, "detect", this));
		}
	}
		
	
	protected virtual IEnumerable<Collider> Detect()
	{
		return Physics.OverlapSphere(Vehicle.Position, DetectionRadius, LayersChecked);
	}
	
	protected virtual void FilterDetected()
	{
		// Materialize the list so that we don't select it twice
		var notIgnored =  _detected.Select( d => d.transform.root.GetComponentInChildren<DetectableObject>() ).Except(_ignoredObjects).ToList();
		_vehicles = notIgnored.OfType<Vehicle>().Where( v => v != null && (v.enabled || _detectDisabledVehicles));
		_obstacles = notIgnored.Where( d => d != null && !(d is Vehicle) );
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
