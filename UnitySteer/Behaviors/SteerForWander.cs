using System.Collections.Generic;
using UnityEngine;
using UnitySteer;
using UnitySteer.Helpers;

/// <summary>
/// Steers a vehicle to wander around
/// </summary>
public class SteerForWander : Steering
{
	float _wanderSide;
	float _wanderUp;
	
	
	protected override Vector3 CalculateForce ()
	{
		float speed = Vehicle.MaxSpeed;

		// random walk WanderSide and WanderUp between -1 and +1
		_wanderSide = OpenSteerUtility.scalarRandomWalk (_wanderSide, speed, -1, +1);
		_wanderUp   = OpenSteerUtility.scalarRandomWalk (_wanderUp,   speed, -1, +1);
		
		// return a pure lateral steering vector: (+/-Side) + (+/-Up)
		Vector3	 result = (transform.right * _wanderSide) + (transform.up * _wanderUp);
		return result;
	}
	
}

