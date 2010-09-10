using UnityEngine;
using UnitySteer.Helpers;

public class Steering : MonoBehaviour, ITick {	
	
	#region Private fields
	/// <summary>
	/// Last force calculated
	/// </summary>
	Vector3 _force = Vector3.zero;
	
	/// <summary>
	/// Cached vehicle
	/// </summary>
	Vehicle _vehicle;
	
	[SerializeField]
	Tick _tick;
	
	[SerializeField]
	float _weight = 1;
	#endregion
	
	
	#region Public properties
	/// <summary>
	/// The force vector calculated by this steering behavior
	/// </summary>
	public Vector3 Force
	{
		get
		{
			if (Tick.ShouldTick())
				_force = CalculateForce();
			return _force;
		}
	}
	
	/// <summary>
	/// Force vector modified by the assigned weight 
	/// </summary>
	public Vector3 WeighedForce
	{
		get {
			return Force * _weight;
		}
	}
	
	/// <summary>
	/// Tick information
	/// </summary>
	public Tick Tick 
	{
		get 
		{
			return _tick;
		}
	}	
	
	/// <summary>
	/// Vehicle that this behavior will influence
	/// </summary>
	public Vehicle Vehicle {
		get { return _vehicle; }
	}
	
	/// <summary>
	/// Weight assigned to this steering behavior
	/// </summary>
	public float Weight {
		get {
			return this._weight;
		}
		set {
			_weight = value;
		}
	}
	#endregion
	
	#region Methods
	protected void Start()
	{
		_vehicle = this.GetComponent<Vehicle>();
	}
	
	/// <summary>
	/// Calculates the force desired by this behavior
	/// </summary>
	/// <returns>
	/// A vector with the desired force <see cref="Vector3"/>
	/// </returns>
	protected virtual Vector3 CalculateForce()
	{
		return Vector3.zero;
	}
	#endregion
}