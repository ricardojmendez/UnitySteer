using UnityEngine;
using UnitySteer;
using UnitySteer.Helpers;

namespace UnitySteer.Base
{

/// <summary>
/// Steers a vehicle to avoid another one
/// </summary>
[AddComponentMenu("UnitySteer/Steer/... for Evasion")]
public class SteerForEvasion : Steering
{
    float _sqrSafetyDistance = 0;
    
	#region Private fields
	[SerializeField]
	Vehicle _menace;

	[SerializeField]
	float _predictionTime;
    
    /// <summary>
    /// Distance at which the behavior will consider itself safe and stop avoiding
    /// </summary>
    [SerializeField]
    float _safetyDistance = 2f;
	#endregion
	
	#region Public properties
	public override bool IsPostProcess 
	{
		get { return true; }
	}

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
	public Vehicle Menace {
		get {
			return this._menace;
		}
		set {
			_menace = value;
		}
	}

    public float SafetyDistance {
        get {
            return this._safetyDistance;
        }
        set {
            _safetyDistance = value;
            _sqrSafetyDistance = _safetyDistance * _safetyDistance;
        }
    }
	#endregion
    
    protected override void Start() {
        base.Start();
        _sqrSafetyDistance = _safetyDistance * _safetyDistance;
    }
	
	protected override Vector3 CalculateForce()
	{
        if (_menace == null || (Vehicle.Position - _menace.Position).sqrMagnitude > _sqrSafetyDistance) {
            return Vector3.zero;
        }
		// offset from this to menace, that distance, unit vector toward menace
		var position = Vehicle.PredictFutureDesiredPosition(_predictionTime);
		Vector3 offset = _menace.Position - position;
		float distance = offset.magnitude;

		float roughTime = distance / _menace.Speed;
		float predictionTime = ((roughTime > _predictionTime) ?
									  _predictionTime :
									  roughTime);

		Vector3 target = _menace.PredictFuturePosition(predictionTime);

		// This was the totality of SteerToFlee
		Vector3 desiredVelocity = position - target;
		return desiredVelocity - Vehicle.DesiredVelocity;		
	}
}
	
}