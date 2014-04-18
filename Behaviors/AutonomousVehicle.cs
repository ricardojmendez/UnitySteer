using UnityEngine;
using UnitySteer;

namespace UnitySteer.Base
{

/// <summary>
/// Vehicle subclass which automatically applies the steering forces from
/// the components attached to the object.  AutonomousVehicle is characterized
/// for the vehicle always moving in the same direction as its forward vector,
/// unlike Bipeds which are able to side-step.
/// </summary>
[AddComponentMenu("UnitySteer/Vehicle/Autonomous")]
public class AutonomousVehicle : TickedVehicle
{
	#region Internal state values
	float _speed = 0;
	#endregion

	/// <summary>
	/// Acceleration in units per second
	/// </summary>
	/// <remarks>>
	/// We could replace this with a curve in the future.  Then again, if you want
	/// that sort of acceleration control you can probably write a post-processing
	/// behavior to handle it.
	/// </remarks>
	[SerializeField]
	float _accelerationRate = 5;

	/// <summary>
	/// Deceleration in units per second
	/// </summary>
	/// <remarks>>
	/// We could replace this with a curve in the future.
	/// </remarks>
	[SerializeField]
	float _decelerationRate = 8;


	
	public override float Speed
	{
		get { return _speed; }
	}

	/// <summary>
	/// Current vehicle velocity
	/// </summary>
	public override Vector3 Velocity
	{
		get
		{
			return Transform.forward * Speed;
		}
		protected set
		{
			throw new System.NotSupportedException("Cannot set the velocity directly on AutonomousVehicle");
		}
	}
	
	#region Speed-related methods
	public override void UpdateOrientationVelocity(Vector3 velocity)
	{
		TargetSpeed = velocity.magnitude;
		OrientationVelocity = Mathf.Approximately(_speed, 0) ? Transform.forward : velocity / _speed;
	}

	protected override Vector3 CalculatePositionDelta(float deltaTime)
	{
		/*
		 * Notice that we clamp the target speed and not the speed itself, 
		 * because the vehicle's maximum speed might just have been lowered
		 * and we don't want its actual speed to suddenly drop.
		 */
		var targetSpeed = Mathf.Clamp(TargetSpeed, 0, MaxSpeed);
		if (Mathf.Approximately(_speed, targetSpeed))
		{
			_speed = targetSpeed;
		}
		else
		{
			var rate = TargetSpeed > _speed ? _accelerationRate : _decelerationRate;
			_speed = Mathf.Lerp(_speed, targetSpeed, deltaTime * rate);
		}

		return Velocity * deltaTime;
	}

	protected override void ZeroVelocity()
	{
		TargetSpeed = 0;
	}
	#endregion
}	

}