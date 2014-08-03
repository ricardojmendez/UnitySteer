using UnityEngine;

namespace UnitySteer.Behaviors
{

/// <summary>
/// Steers a vehicle to match velocity (speed + heading) with detected neighbors
/// </summary>
/// <remarks> This behavior serves a similar purpose to SteerForAlignment. However,
/// SteerForAlignment has the deficiency that it returns a pure orientation vector,
/// whereas SteerForEvasion and SteerForCohesion return a *distance* from the vehicle's
/// position.  Steering to match the neighbors velocity is more consistent with the others.
/// </remarks>
/// <seealso cref="SteerForAlignment"/>
[AddComponentMenu("UnitySteer/Steer/... for Matching Velocity")]
[RequireComponent(typeof(SteerForNeighborGroup))]
public class SteerForMatchingVelocity : SteerForNeighbors
{
	public override Vector3 CalculateNeighborContribution(Vehicle other)
	{
		// accumulate sum of neighbors' velocities
		return other.Velocity;
	}
}

}
