using UnityEngine;
using System.Collections;

public class SteerForTarget : Steering {
	
	/// <summary>
	/// Target the behavior will aim for
	/// </summary>
	public Transform Target;

	/// <summary>
	/// Should the force be calculated?
	/// </summary>
	/// <returns>
	/// A <see cref="Vector3"/>
	/// </returns>
	protected override Vector3 CalculateForce()
	{
		var force = Vector3.zero;
		if (Target == null)
		{
			Destroy(this);
			throw new System.Exception("SteerForTarget need a target transform. Dying.");
		}
		else
		{
			/*
			 * First off, we calculate how far we are from the target, If this
			 * distance is smaller than the configured vehicle radius, we tell
			 * the vehicle to stop.
			 */
            float d = Vector3.Distance(transform.position, Target.position);
            float r = Vehicle.Radius;
            if (d < r)
			{
				return Vector3.zero;
			}
			
			/*
			 * But suppose we still have some distance to go. The first step
			 * then would be calculating the steering force necessary to orient
			 * ourselves to and walk to that point.  The steerForSeek function
			 * takes into account values luke the MaxForce to apply and the 
			 * vehicle's MaxSpeed, and returns a steering vector.
			 * 
			 * It doesn't apply the steering itself, simply returns the value so
			 * we can continue operating on it.
			 */
			force = Target.position - transform.position - Vehicle.Velocity;
		}		
		return force;
	}
}