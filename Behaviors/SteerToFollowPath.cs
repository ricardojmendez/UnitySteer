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
public class SteerToFollowPath : Steering
{
	
	#region Private fields
	FollowDirection _direction = FollowDirection.Forward;
	float _predictionTime = 2f;
	Pathway _path;
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
		if (_path == null) 
			return Vector3.zero;
		
		// our goal will be offset from our path distance by this amount
		float pathDistanceOffset = (int)_direction * _predictionTime * Vehicle.Speed;
		
		// predict our future position
		Vector3 futurePosition = Vehicle.PredictFuturePosition(_predictionTime);
		
		// measure distance along path of our current and predicted positions
		float nowPathDistance = _path.mapPointToPathDistance (Vehicle.Position);
		float futurePathDistance = _path.mapPointToPathDistance (futurePosition);
		
		// are we facing in the correction direction?
		bool rightway = ((pathDistanceOffset > 0) ? (nowPathDistance < futurePathDistance) : (nowPathDistance > futurePathDistance));
		
		// find the point on the path nearest the predicted future position
		// XXX need to improve calling sequence, maybe change to return a
		// XXX special path-defined object which includes two Vector3s and a 
		// XXX bool (onPath,tangent (ignored), withinPath)
		mapReturnStruct tStruct = new mapReturnStruct ();
		_path.mapPointToPath (futurePosition, ref tStruct);
		
		
		// no steering is required if (a) our future position is inside
		// the path tube and (b) we are facing in the correct direction
		if ((tStruct.outside < 0) && rightway) {
			// all is well, return zero steering
			return Vector3.zero;
		} else {
			// otherwise we need to steer towards a target point obtained
			// by adding pathDistanceOffset to our current path position
			
			float targetPathDistance = nowPathDistance + pathDistanceOffset;
			Vector3 target = _path.mapPathDistanceToPoint (targetPathDistance);
			
			// return steering to seek target on path
			return Vehicle.GetSeekVector(target);
		}
	}
}
