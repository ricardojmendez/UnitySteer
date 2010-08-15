using UnityEngine;

/// <summary>
/// Steers a vehicle in alignment with detected neighbors
/// </summary>
public class SteerForAlignment : SteerForNeighbors
{
	protected override Vector3 CalculateNeighborContribution(Vehicle other)
	{
		// accumulate sum of neighbor's heading
		return other.transform.forward;;
	}
}
