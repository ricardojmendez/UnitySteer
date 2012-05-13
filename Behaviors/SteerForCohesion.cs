#define DEBUG_COMFORT_DISTANCE
using UnityEngine;

/// <summary>
/// Steers a vehicle to remain in cohesion with neighbors
/// </summary>
[AddComponentMenu("UnitySteer/Steer/... for Cohesion")]
public class SteerForCohesion : SteerForNeighbors
{
	public float comfortDistance = 3;
	
	protected override Vector3 CalculateNeighborContribution(Vehicle other)
	{
		// accumulate sum of forces leading us towards neighbor's positions
		var distance = other.Position - Vehicle.Position;
		if (distance.magnitude < comfortDistance)
			return Vector3.zero;
		return distance;
	}
	
	void OnDrawGizmos() {
		#if DEBUG_COMFORT_DISTANCE
		Gizmos.color = Color.magenta;
		Gizmos.DrawWireSphere(transform.position, comfortDistance);
		#endif		
	}
}

