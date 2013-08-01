using UnityEngine;

/// <summary>
/// Steers a vehicle in alignment with detected neighbors
/// </summary>
[AddComponentMenu("UnitySteer/Steer/... for Alignment")]
[RequireComponent(typeof(SteerForNeighborGroup))]
public class SteerForAlignment : SteerForNeighbors
{
	public override Vector3 CalculateNeighborContribution(Vehicle other)
	{
		// accumulate sum of neighbor's heading
		return other.transform.forward;;
	}
}
