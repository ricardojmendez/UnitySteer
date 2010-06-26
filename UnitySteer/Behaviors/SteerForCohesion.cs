using System.Collections.Generic;
using UnityEngine;
using UnitySteer.Helpers;


/// <summary>
/// Steers a vehicle to remain in cohesion with neighbors
/// </summary>
public class SteerForCohesion : SteerForNeighbors
{
	protected override Vector3 CalculateForce ()
	{
		// steering accumulator and count of neighbors, both initially zero
		Vector3 steering = Vector3.zero;
		int neighbors = 0;
		
		var flock = new List<Vehicle>();
		
		foreach (var d in Vehicle.Radar.Detected)
		{
			var c = d.GetComponent<Vehicle>();
			if (c != null)
				flock.Add(c);
		}
		
		// for each of the other vehicles...
		for (int i = 0; i < flock.Count; i++)
		{
			Vehicle other = flock[i];
			if (VehicleHelper.InNeighborhood (this.Vehicle, other, Vehicle.Radius * 3, Radius, AngleCos)) 
			{
				// accumulate sum of neighbor's positions
				steering += other.transform.position;

				// count neighbors
				neighbors++;
			}
		}

		// divide by neighbors, subtract off current position to get error-
		// correcting direction, then normalize to pure direction
		if (neighbors > 0)
		{
			steering = ((steering / (float)neighbors) - transform.position);
			steering.Normalize();
		}

		return steering;
	
	}
}

