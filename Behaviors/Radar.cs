using System.Linq;
using C5;
using UnityEngine;
using UnitySteer;
using UnitySteer.Helpers;


/// <summary>
/// Base class for radars
/// </summary>
/// <remarks>Different radars will implement their own detection styles, from
/// "pinging" every so often with Physics.OverlapSphere to handling visibility
/// OnTriggerEnter/Exit</remarks>
public class Radar: MonoBehaviour, ITick {
	#region Private properties
	SteeringEventHandler<Radar> _onDetected;
	
	[SerializeField]
	Tick _tick;
	
	[SerializeField]
	LayerMask _obstacleLayer;

	[SerializeField]
	LayerMask _layersChecked;
		
	
	IList<Collider> _detected;
	IList<Vehicle> _vehicles = new ArrayList<Vehicle>();
	IList<Obstacle> _obstacles = new ArrayList<Obstacle>();
	
	ObstacleFactory _obstacleFactory = null;
	
	Vehicle _vehicle;
	#endregion
	
	
	#region Public properties
	/// <summary>
	/// List of currently detected neighbors
	/// </summary>
	public IList<Collider> Detected {
		get {
			ExecuteRadar();
			return _detected;
		}
	}
	
	/// <summary>
	/// List of obstacles detected by the radar
	/// </summary>
	public IList<Obstacle> Obstacles {
		get {
			ExecuteRadar();
			return new GuardedList<Obstacle>(_obstacles);
		}

	}
	
	public SteeringEventHandler<Radar> OnDetected {
		get {
			return this._onDetected;
		}
		set {
			_onDetected = value;
		}
	}

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
	public IList<Vehicle> Vehicles {
		get {
			ExecuteRadar();
			return new GuardedList<Vehicle>(_vehicles);
		}
	}

	/// <summary>
	/// Layers for objects considered obstacles
	/// </summary>
	public LayerMask ObstacleLayer {
		get {
			return this._obstacleLayer;
		}
		set {
			_obstacleLayer = value;
		}
	}
	
	/// <summary>
	/// Delegate for the method used to create obstacles
	/// </summary>
	/// <remarks>This delegate must be set by any steering behavior that
	/// wishes to obtain a list of stationary obstacles to steer away from.
	/// Notice that this means we can only have one type of obstacle detected,
	/// which is just fine for now but we may want to review it in the future.
	/// </remarks>
	public ObstacleFactory ObstacleFactory {
		get {
			return this._obstacleFactory;
		}
		set {
			_obstacleFactory = value;
		}
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

	/// <summary>
	/// Tick information
	/// </summary>
	public Tick Tick {
		get {
			return this._tick;
		}
	}
	#endregion
	
	#region Methods
	protected virtual void Awake() {
		_vehicle = GetComponent<Vehicle>();	
	}
	
	
	void ExecuteRadar()
	{
		if (_tick.ShouldTick()) {
			_detected = Detect();
			FilterDetected();
			if (_onDetected != null)
				_onDetected(new SteeringEvent<Radar>(null, "detect", this));
		}
	}
	
	protected virtual IList<Collider> Detect()
	{
		return new ArrayList<Collider>();
	}
	
	protected virtual void FilterDetected()
	{
		_vehicles.Clear();
		_obstacles.Clear();
		foreach (var other in _detected)
		{
			var vehicle = other.gameObject.GetComponent<Vehicle>();
			if (vehicle != null && other.gameObject != this.gameObject)
			{
				_vehicles.Add(vehicle);
			}
			if (ObstacleFactory != null && ((1 << other.gameObject.layer & ObstacleLayer) > 0))
			{
				var obstacle = ObstacleFactory(other.gameObject);
				_obstacles.Add (obstacle);
			}
		}
	}
	#endregion
}
