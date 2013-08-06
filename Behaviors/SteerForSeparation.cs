using UnityEngine;

/// <summary>
/// Steers a vehicle to keep separate from neighbors
/// </summary>
[AddComponentMenu("UnitySteer/Steer/... for Separation")]
[RequireComponent(typeof(SteerForNeighborGroup))]
public class SteerForSeparation : SteerForNeighbors
{
	#region Methods
	public override Vector3 CalculateNeighborContribution(Vehicle other)
	{
		// add in steering contribution
		// (opposite of the offset direction, divided once by distance
		// to normalize, divided another time to get 1/d falloff)
		Vector3 offset = other.Position - Vehicle.Position;
		Vector3 steering = (offset / -offset.sqrMagnitude);	
		return steering;
	}
	#endregion
}

