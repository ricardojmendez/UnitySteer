using System.Collections.Generic;
using UnityEngine;
using UnitySteer;
using UnitySteer.Helpers;


/// <summary>
/// Steers a vehicle in alignment with detected neighbors
/// </summary>
public class SteerForAlignment : SteerForNeighbors
{
	protected override Vector3 CalculateForce()
	{
		// steering accumulator and count of neighbors, both initially zero
		Vector3 steering=Vector3.zero;
		int neighbors = 0;
		
		// for each of the other vehicles...
		for (int i = 0; i < Vehicle.Radar.Vehicles.Count; i++)
		{
			Vehicle other = Vehicle.Radar.Vehicles[i];
			if (Vehicle.IsInNeighborhood (other, Vehicle.Radius * 3, Radius, AngleCos))
			{
				// accumulate sum of neighbor's heading
				steering += other.transform.forward;

				// count neighbors
				neighbors++;
			}
		}

		// divide by neighbors, subtract off current heading to get error-
		// correcting direction, then normalize to pure direction
		if (neighbors > 0)
		{
			steering = ((steering / (float)neighbors) - transform.forward);
			steering.Normalize();
		}

		return steering;
	}
}
