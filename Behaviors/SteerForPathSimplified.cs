using System.Collections;
using UnityEngine;
using UnitySteer;

/// <summary>
/// Steers a vehicle to follow a path
/// </summary>
/// <remarks>
/// Based on SteerToFollowPath.
/// </remarks>
[AddComponentMenu("UnitySteer/Steer/... for PathSimplified")]
public class SteerForPathSimplified : Steering
{
	
	#region Private fields
	[SerializeField]
	float _predictionTime = 1.5f;
	
	[SerializeField]
	float _minSpeedToConsider = 0.25f;
	
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
	/// Minimum speed to consider when predicting the future position. If the
	/// vehicle's speed is under this value, estimates will instead be done
	/// at this value plus the prediction time.
	/// </summary>
	public float MinSpeedToConsider {
		get {
			return this._minSpeedToConsider;
		}
		set {
			_minSpeedToConsider = value;
		}
	}

	/// <summary>
	/// Path to follow
	/// </summary>
	public IPathway Path { get; set; }
	#endregion

	/// <summary>
	/// Should the force be calculated?
	/// </summary>
	/// <returns>
	/// A <see cref="Vector3"/>
	/// </returns>
	protected override Vector3 CalculateForce ()
	{
		if (Path == null || Path.SegmentCount < 2) 
		{
			return Vector3.zero;
		}
		
		// If the vehicle's speed is 0, use a low speed for future position
		// calculation. Otherwise the vehicle will remain where it is if he
		// starts within the path, because its current position matches its
		// future path position
		float speed = (Vehicle.Speed > _minSpeedToConsider) ? Vehicle.Speed : _minSpeedToConsider + _predictionTime;
		
		// our goal will be offset from our path distance by this amount
		float pathDistanceOffset = _predictionTime * speed;
		
		// measure distance along path of our current and predicted positions
		float currentPathDistance = Path.MapPointToPathDistance (Vehicle.Position);
		
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
		var target = Path.MapPathDistanceToPoint (targetPathDistance);
		
		/*
		 * Return steering to seek target on path.
		 * 
		 * If you set the considerVelocity parameter to true, it'll slow
		 * down at each target to try to ease its arrival, which will 
		 * likely cause it to come to a stand still at low prediction
		 * times.
		 *
		 */
		var seek = Vehicle.GetSeekVector(target, false);
		
		if (seek == Vector3.zero && targetPathDistance <= Path.TotalPathLength)
		{
			/*
			 * If we should not displace but still have some distance to go,
			 * that means that we've encountered an edge case: a relatively low
			 * vehicle speed and short prediction range, combined with a path
			 * that twists. In that case, it's possible that the predicted future
			 * point just around the bend is still within the vehicle's arrival
			 * radius.  In that case, aim a bit further beyond the vehicle's 
			 * arrival radius so that it can continue moving.
			 * 
			 * TODO: Consider simply adding the arrivalradius displacement to
			 * where we're aiming to from the get go. Might leave as is, considering
			 * that this is supposed to be just a sample behavior.
			 */
			target = Path.MapPathDistanceToPoint(targetPathDistance + 1.5f * Vehicle.ArrivalRadius);
			seek = Vehicle.GetSeekVector(target, false);
		}
		
		return seek;
	}
	
	protected void OnDrawGizmosSelected()
	{
		if (Path != null)
		{
			Path.DrawGizmos();
		}
	}
}
