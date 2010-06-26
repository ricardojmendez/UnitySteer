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
	
	ICollection<Collider> _detected;
	
	[SerializeField]
	LayerMask _layersChecked;
	#endregion
	
	
	#region Public properties
	/// <summary>
	/// List of currently detected neighbors
	/// </summary>
	public ICollection<Collider> Detected {
		get {
			if (_tick.ShouldTick)
				_detected = Detect();
			return _detected;
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
	protected virtual ICollection<Collider> Detect()
	{
		return new List<Collider>();
	}
	#endregion
}
