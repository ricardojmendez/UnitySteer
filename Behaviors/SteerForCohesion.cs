using UnityEngine;

/// <summary>
/// Steers a vehicle to remain in cohesion with neighbors
/// </summary>
[AddComponentMenu("UnitySteer/Steer/... for Cohesion")]
public class SteerForCohesion : SteerForNeighbors
{
	protected override Vector3 CalculateNeighborContribution(Vehicle other)
	{
		// accumulate sum of forces leading us towards neighbor's positions
		return other.Position - Vehicle.Position;
	}
}

