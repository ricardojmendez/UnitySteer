using UnityEngine;
using System.Collections;
using UnitySteer.Helpers;

public class Steering : MonoBehaviour {
	
	/// <summary>
	/// Last force calculated
	/// </summary>
	private Vector3 _force = Vector3.zero;
	
	Vehicle _vehicle;
	
	[SerializeField]
	private Tick _tick;
	
	
	/// <summary>
	/// The force vector calculated by this steering behavior
	/// </summary>
	public Vector3 Force
	{
		get
		{
			if (Tick.ShouldTick)
				_force = CalculateForce();
			return _force;
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
	
	public Vehicle Vehicle {
		get { return _vehicle; }
	}
	
	
	void Start()
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
	
	
	
	
}