using System.Collections;
using UnityEngine;
using UnitySteer;

public enum FollowDirection: int {
	Forward = +1,
	Back = -1
}

/// <summary>
/// Steers a vehicle to follow a path
/// </summary>
/// <remarks>
/// RJM:
/// 
/// This is pretty much a direct conversion of the original OpenSteer 
/// steerToFollowPath method, minus some global variable use. It looks for 
/// the point along path that's nearest to the vehicle's future position. 
/// As such, it won't strictly follow a path that  loops over itself (or 
/// even one were two segments come really close), and will look like it's 
/// taking a shortcut if the prediction time is too large.
/// 
/// See also SteerForPathSimplified.
/// </remarks>
[AddComponentMenu("UnitySteer/Steer/... to Follow Path")]
public class SteerToFollowPath : Steering
{
	
	#region Private fields
	FollowDirection _direction = FollowDirection.Forward;
	float _predictionTime = 2f;
	IPathway _path;
	#endregion
	
	
	#region Public properties
	/// <summary>
	/// Direction to follow the path on
	/// </summary>
	public FollowDirection Direction {
		get {
			return this._direction;
		}
		set {
			_direction = value;
		}
	}
	
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
	public IPathway Path {
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
			return Vector3.zero;
		
		// our goal will be offset from our path distance by this amount
		float pathDistanceOffset = (int)_direction * _predictionTime * Vehicle.Speed;
		
		// predict our future position
		Vector3 futurePosition = Vehicle.PredictFuturePosition(_predictionTime);
		
		// measure distance along path of our current and predicted positions
		float nowPathDistance = _path.MapPointToPathDistance (Vehicle.Position);
		float futurePathDistance = _path.MapPointToPathDistance (futurePosition);
		
		// are we facing in the correction direction?
		bool rightway = ((pathDistanceOffset > 0) ? (nowPathDistance < futurePathDistance) : (nowPathDistance > futurePathDistance));
		
		// find the point on the path nearest the predicted future position
		var tStruct = new PathRelativePosition ();
		_path.MapPointToPath (futurePosition, ref tStruct);
		
		
		// no steering is required if (a) our future position is inside
		// the path tube and (b) we are facing in the correct direction
		if ((tStruct.outside < 0) && rightway) 
		{
			// TODO Evaluate. This assumes the vehicle has inertia, and would stop if inside the path tube
			return Vector3.zero;
		} else 
		{
			/*
			 * Otherwise we need to steer towards a target point obtained
			 * by adding pathDistanceOffset to our current path position.
			 * 
			 * Notice that this method does not steer for the point in the
			 * path that is closest to our future position, which is why 
			 * the return value of MapPointToPath is ignored above. Instead,
			 * it estimates how far the vehicle will move in units, and then
			 * aim for the point in the path that is that many units away
			 * from our current path position _in path length_.   This means 
			 * that it adds up the segment lengths and aims for the point 
			 * that is N units along the length of the path, which can imply
			 * bends and turns and is not a straight vector projected away
			 * from our position.
			 * 
			 * This also means that having too high a prediction time will
			 * have the effect of the agent seemingly approaching the path
			 * in an elliptical manner.
			 */
			float targetPathDistance = nowPathDistance + pathDistanceOffset;
			var target = _path.MapPathDistanceToPoint (targetPathDistance);
			
			// return steering to seek target on path
			var seek = Vehicle.GetSeekVector(target);
			return seek;
		}
	}
}
