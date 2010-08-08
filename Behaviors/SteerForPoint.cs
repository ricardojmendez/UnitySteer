using UnityEngine;

public class SteerForPoint: Steering
{
	/// <summary>
	/// The target point.
	/// </summary>
 	public Vector3 TargetPoint;
	
	
	void Awake() {
		if (TargetPoint == Vector3.zero)
			TargetPoint = transform.position;
	}
	
	/// <summary>
	/// Calculates the force to apply to the vehicle to reach a point
	/// </summary>
	/// <returns>
	/// A <see cref="Vector3"/>
	/// </returns>
	protected override Vector3 CalculateForce()
	{
		return Vehicle.GetSeekVector(TargetPoint);
	}
}


