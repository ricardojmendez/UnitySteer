using UnityEngine;
using System.Collections;

public class SteerForTarget : Steering {
	
	/// <summary>
	/// Target the behavior will aim for
	/// </summary>
	public Transform Target;
	
	
	void Awake()
	{
		if (Target == null)
		{
			Destroy(this);
			throw new System.Exception("SteerForTarget need a target transform. Dying.");
		}
	}

	/// <summary>
	/// Should the force be calculated?
	/// </summary>
	/// <returns>
	/// A <see cref="Vector3"/>
	/// </returns>
	protected override Vector3 CalculateForce()
	{
		return Vehicle.GetSeekVector(Target.position);
	}
}