using UnityEngine;
using UnitySteer.Helpers;

/// <summary>
/// Trivial example that simply makes the vehicle move towards its forward vector
/// </summary>
[AddComponentMenu("UnitySteer/Steer/... for Forward")]
public class SteerForForward : Steering
{
	
	/// <summary>
	/// Calculates the force to apply to the vehicle to reach a point
	/// </summary>
	/// <returns>
	/// A <see cref="Vector3"/>
	/// </returns>
	protected override Vector3 CalculateForce()
	{
		return Vehicle.Transform.forward;
	}
}
