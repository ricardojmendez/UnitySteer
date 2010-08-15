using UnityEngine;
using UnitySteer;
using UnitySteer.Helpers;

/// <summary>
/// Steers a vehicle to wander around
/// </summary>
public class SteerForWander : Steering
{
	#region Private fields
	float _wanderSide;
	float _wanderUp;
	
	[SerializeField]
	float _maxLatitudeSide = 2;
	[SerializeField]
	float _maxLatitudeUp = 2;
	#endregion
	
	
	#region Public properties
	/// <summary>
	/// Maximum latitude to use for the random scalar walk on the side
	/// </summary>
	public float MaxLatitudeSide {
		get {
			return this._maxLatitudeSide;
		}
		set {
			_maxLatitudeSide = value;
		}
	}

	/// <summary>
	/// Maximum latitude to use for the random scalar walk on the up vector
	/// </summary>
	public float MaxLatitudeUp {
		get {
			return this._maxLatitudeUp;
		}
		set {
			_maxLatitudeUp = value;
		}
	}
	#endregion

	
	protected override Vector3 CalculateForce ()
	{
		float speed = Vehicle.MaxSpeed;

		// random walk WanderSide and WanderUp between -1 and +1
		_wanderSide = OpenSteerUtility.scalarRandomWalk (_wanderSide, speed, -_maxLatitudeSide, _maxLatitudeSide);
		_wanderUp   = OpenSteerUtility.scalarRandomWalk (_wanderUp,   speed, -_maxLatitudeUp,   _maxLatitudeUp);
		
		// return a pure lateral steering vector: (+/-Side) + (+/-Up)
		Vector3	 result = (transform.right * _wanderSide) + (transform.up * _wanderUp);
		return result;
	}
	
}

