using UnityEngine;
using UnitySteer;
using UnitySteer.Helpers;

/// <summary>
/// Steers a vehicle to avoid another CharacterController (very basic future position prediction)
/// </summary>
[AddComponentMenu("UnitySteer/Steer/... for Character Evasion")]
public class SteerForCharacterEvasion : Steering
{
	#region Private fields
	[SerializeField]
	CharacterController _menace;

	[SerializeField]
	float _predictionTime;
	#endregion
	
	#region Public properties
	/// <summary>
	/// How many seconds to look ahead for position prediction
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
	/// Vehicle menace
	/// </summary>
	public CharacterController Menace {
		get {
			return this._menace;
		}
		set {
			_menace = value;
		}
	}
	#endregion
	
	protected override Vector3 CalculateForce()
	{
		// offset from this to menace, that distance, unit vector toward menace
		Vector3 offset = _menace.transform.position - Vehicle.Position;
		float distance = offset.magnitude;
		
		float roughTime = distance / _menace.velocity.magnitude;
		float predictionTime = ((roughTime > _predictionTime) ?
									  _predictionTime :
									  roughTime);
		
		Vector3 target = _menace.transform.position + (_menace.velocity * predictionTime);

		// This was the totality of SteerToFlee
		Vector3 desiredVelocity = Vehicle.Position - target;
		return desiredVelocity - Vehicle.Velocity;		
	}
	
}