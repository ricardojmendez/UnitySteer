#define TRACE_ADJUSTMENTS
using UnityEngine;
using UnitySteer;
using System.Linq;
using TickedPriorityQueue;

/// <summary>
/// Vehicle subclass oriented towards autonomous bipeds, which have a movement
/// vector which can be separate from their forward vector.   
/// </summary>
[AddComponentMenu("UnitySteer/Vehicle/Biped")]
public class Biped : TickedVehicle
{
	#region Internal state values
	/// <summary>
	/// The magnitude of the last velocity vector assigned to the vehicle 
	/// </summary>
	float _speed = 0;

	/// <summary>
	/// The biped's current velocity vector
	/// </summary>
	Vector3 _velocity;
	#endregion
	
	public	override bool CanMove
	{
		set 
		{ 
			base.CanMove = value;
			if (!CanMove)
			{
				Velocity = Vector3.zero;
			}
		}
	}
	
	public override float Speed
	{
		get
		{ 
			return Speedometer == null ? _speed : Speedometer.Speed; 
		}
		set
		{
			throw new System.NotSupportedException("Cannot set the speed directly on Bipeds");
		}
	}

	/// <summary>
	/// Current vehicle velocity
	/// </summary>
	public override Vector3 Velocity
	{
		get { return _velocity; }
		set 
		{ 
			_velocity = Vector3.ClampMagnitude(value, MaxSpeed);
			_speed = _velocity.magnitude;
			OrientationVelocity = _speed != 0 ? _velocity / _speed : Vector3.zero;
		}
	}

	
	#region Methods
	protected override void OnEnable()
	{
		base.OnEnable();
		Velocity = Vector3.zero;
	}
	
	
	protected override void RecordCalculatedVelocity(Vector3 velocity)
	{
		Velocity = velocity;
	}

	protected override Vector3 CalculatePositionDelta(float deltaTime)
	{
		return Velocity * deltaTime;
	}

	protected override void ZeroVelocity()
	{
		Velocity = Vector3.zero;
	}
	#endregion
	
}


