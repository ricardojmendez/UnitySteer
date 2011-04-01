using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnitySteer;
using UnitySteer.Helpers;


/// <summary>
/// Base class for radars
/// </summary>
/// <remarks>Different radars will implement their own detection styles, from
/// "pinging" every so often with Physics.OverlapSphere to handling visibility
/// OnTriggerEnter/Exit</remarks>
public class Radar: MonoBehaviour {
	#region Private properties
	
	[SerializeField]
	bool _detectDisabledVehicles;
	
	[SerializeField]
	LayerMask _layersChecked;
		
	
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
	}
	
	public void Update()
	{
		_detected = Detect();
		FilterDetected();
		if (OnDetected != null)
			OnDetected(new SteeringEvent<Radar>(null, "detect", this));
	}
		
	
	protected virtual IEnumerable<Collider> Detect()
	{
		return new List<Collider>();
	}
	
	protected virtual void FilterDetected()
	{
		_vehicles = _detected.Select( c => c.gameObject.GetComponent<Vehicle>() ).Where( v => v != null && v != _vehicle && (v.enabled || _detectDisabledVehicles) && !_ignoredObjects.Contains(v));
		_obstacles = _detected.Select( d => d.gameObject.GetComponent<DetectableObject>() ).Where( d => d != null && !(d is Vehicle) && !_ignoredObjects.Contains(d));
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
}
