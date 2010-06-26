using System.Collections.Generic;
using UnityEngine;
using UnitySteer.Helpers;

/// <summary>
/// Steers a vehicle to keep separate from neighbors
/// </summary>
public class SteerForSeparation : SteerForNeighbors
{
	#region Methods
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
		for (int i = 0; i < flock.Count; i++) {
			Vehicle other = flock[i];
			if (VehicleHelper.InNeighborhood (this.Vehicle, other, Vehicle.Radius * 3, Radius, AngleCos)) {
				// add in steering contribution
				// (opposite of the offset direction, divided once by distance
				// to normalize, divided another time to get 1/d falloff)
				Vector3 offset = (other).transform.position - transform.position;
				float distanceSquared = Vector3.Dot (offset, offset);
				steering += (offset / -distanceSquared);
				
				// count neighbors
				neighbors++;
			}
		}
		
		// divide by neighbors, then normalize to pure direction
		if (neighbors > 0) {
			steering = (steering / (float)neighbors);
			steering.Normalize();
		}
		
		return steering;
	}
	#endregion
}

