using System.Collections;
using UnityEngine;
using UnitySteer;

/// <summary>
/// Steers a vehicle to follow a path
/// </summary>
/// <remarks>
/// Based on SteerToFollowPath.
/// </remarks>
public class SteerForPathSimplified : Steering
{
	
	#region Private fields
	[SerializeField]
	float _predictionTime = 1f;
	
	Pathway _path;
	#endregion
	
	
	#region Public properties
	/// <summary>
	/// How far ahead to estimate our position
	/// </summary>
	public float PredictionTime {
		get {
			return this._predictionTime;
		}
		set {
			_predictionTime = value;
		}
	}
	
	/// <summary>
	/// Path to follow
	/// </summary>
	public Pathway Path {
		get {
			return this._path;
		}
		set {
			_path = value;
		}
	}

	#endregion

	/// <summary>
	/// Should the force be calculated?
	/// </summary>
	/// <returns>
	/// A <see cref="Vector3"/>
	/// </returns>
	protected override Vector3 CalculateForce ()
	{
		if (_path == null || _path.SegmentCount < 2) 
		{
			return Vector3.zero;
		}
		
		// our goal will be offset from our path distance by this amount
		float pathDistanceOffset = _predictionTime * Vehicle.Speed;
		
		// measure distance along path of our current and predicted positions
		float currentPathDistance = _path.MapPointToPathDistance (Vehicle.Position);
		
		/*
		 * Otherwise we need to steer towards a target point obtained
		 * by adding pathDistanceOffset to our current path position.
		 * 
		 * Notice that this method does not steer for the point in the
		 * path that is closest to our future position, which is why 
		 * we don't calculate the closest point in the path to our future
		 * position. 
		 * 
		 * Instead, it estimates how far the vehicle will move in units, 
		 * and then aim for the point in the path that is that many units 
		 * away from our current path position _in path length_.   This 
		 * means that it adds up the segment lengths and aims for the point 
		 * that is N units along the length of the path, which can imply
		 * bends and turns and is not a straight vector projected away
		 * from our position.
		 */
		float targetPathDistance = currentPathDistance + pathDistanceOffset;
		var target = _path.MapPathDistanceToPoint (targetPathDistance);
		
		// return steering to seek target on path
		var seek = Vehicle.GetSeekVector(target, false);
		

		
		return seek;
	}
}
