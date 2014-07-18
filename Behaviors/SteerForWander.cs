using UnityEngine;

namespace UnitySteer.Base
{

/// <summary>
/// Steers a vehicle to wander around
/// </summary>
[AddComponentMenu("UnitySteer/Steer/... for Wander")]
public class SteerForWander : Steering
{
	#region Private fields
	float _wanderSide;
	float _wanderUp;
	
	[SerializeField]
	float _maxLatitudeSide = 2;
	[SerializeField]
	float _maxLatitudeUp = 2;
    
    /// <summary>
	/// The smooth rate per second to apply to the random walk value during blending.
    /// </summary>
    [SerializeField]
    float _smoothRate = 0.05f;
	#endregion
	
	#region Public properties
	/// <summary>
	/// Maximum latitude to use for the random scalar walk on the side
	/// </summary>
	public float MaxLatitudeSide {
		get {
			return _maxLatitudeSide;
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
			return _maxLatitudeUp;
		}
		set {
			_maxLatitudeUp = value;
		}
	}
	#endregion

	
	protected override Vector3 CalculateForce()
	{
		float speed = Vehicle.MaxSpeed;

		// random walk WanderSide and WanderUp between the specified latitude
        var randomSide = OpenSteerUtility.ScalarRandomWalk(_wanderSide, speed, -_maxLatitudeSide, _maxLatitudeSide);
        var randomUp = OpenSteerUtility.ScalarRandomWalk(_wanderUp, speed, -_maxLatitudeUp, _maxLatitudeUp);
        _wanderSide = Mathf.Lerp(_wanderSide, randomSide, _smoothRate * Vehicle.DeltaTime);
        _wanderUp = Mathf.Lerp(_wanderUp, randomUp, _smoothRate * Vehicle.DeltaTime);

		Vector3	 result = (Vehicle.Transform.right * _wanderSide) + (Vehicle.Transform.up * _wanderUp) + Vehicle.Transform.forward;
		return result;
	}	
}

}

