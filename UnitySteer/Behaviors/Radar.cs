using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnitySteer.Helpers;


/// <summary>
/// Base class for radars
/// </summary>
/// <remarks>Different radars will implement their own detection styles, from
/// "pinging" every so often with Physics.OverlapSphere to handling visibility
/// OnTriggerEnter/Exit</remarks>
public class Radar: MonoBehaviour, ITick {
	#region Private properties
	[SerializeField]
	Tick _tick;
	
	IList<Collider> _detected;
	List<Vehicle> _vehicles = new List<Vehicle>();
	
	[SerializeField]
	LayerMask _layersChecked;
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
	/// List of vehicles detected among the colliders
	/// </summary>
	public IList<Vehicle> Vehicles {
		get {
			ExecuteRadar();
			return _vehicles.AsReadOnly();
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
	void ExecuteRadar()
	{
		if (_tick.ShouldTick) {
			_detected = Detect();
			FilterDetected();
		}
	}
	
	protected virtual IList<Collider> Detect()
	{
		return new List<Collider>();
	}
	
	protected virtual void FilterDetected()
	{
		_vehicles = _detected.Select( x => x.gameObject.GetComponent<Vehicle>() ).ToList<Vehicle>();
		_vehicles.RemoveAll( x => x == null);
	}
	#endregion
}
