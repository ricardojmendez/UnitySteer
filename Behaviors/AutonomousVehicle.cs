using UnityEngine;
using UnitySteer;

/// <summary>
/// Vehicle subclass which automatically applies the steering forces from
/// the components attached to the object.
/// </summary>
public class AutonomousVehicle: Vehicle
{
	#region Internal state values
	Vector3 _smoothedAcceleration;
	Rigidbody _rigidbody;
	CharacterController _characterController;
	#endregion
	
	
	#region Methods
	void Start()
	{
		_rigidbody = GetComponent<Rigidbody>();
		_characterController = GetComponent<CharacterController>();
	}
		
	
	void FixedUpdate()
	{
		var force = Vector3.zero;

		Profiler.BeginSample("Calculating forces");

		foreach (var steering in Steerings)
		{
			if (steering.enabled)
				force  += steering.WeighedForce;
		}

		Profiler.EndSample();

		ApplySteeringForce(force, Time.fixedDeltaTime);
	}
	
	/// <summary>
	/// Applies a steering force to this vehicle
	/// </summary>
	/// <param name="force">
	/// A force vector to apply<see cref="Vector3"/>
	/// </param>
	/// <param name="elapsedTime">
	/// How long has elapsed since the last update<see cref="System.Single"/>
	/// </param>
	private void ApplySteeringForce(Vector3 force, float elapsedTime)
	{
		if (MaxForce == 0 || MaxSpeed == 0 || elapsedTime == 0)
		{
			return;
		}

		// enforce limit on magnitude of steering force
		Vector3 clippedForce = Vector3.ClampMagnitude(force, MaxForce);

		// compute acceleration and velocity
		Vector3 newAcceleration = (clippedForce / Mass);

		if (newAcceleration.sqrMagnitude == 0 && !HasInertia)
		{
			Speed = 0;
		}

		Vector3 newVelocity = Velocity;
		
		/*
			Damp out abrupt changes and oscillations in steering acceleration
			(rate is proportional to time step, then clipped into useful range)
			
			The lower the smoothRate parameter, the more noise there is
			likely to be in the movement.
		 */
		_smoothedAcceleration = OpenSteerUtility.blendIntoAccumulator(0.4f,
									newAcceleration,
									_smoothedAcceleration);

		// Euler integrate (per frame) acceleration into velocity
		newVelocity += _smoothedAcceleration * elapsedTime;

		// enforce speed limit
		newVelocity = Vector3.ClampMagnitude(newVelocity, MaxSpeed);

		if (IsPlanar)
		{
			newVelocity.y = Velocity.y;
		}

		// update Speed
		Speed = newVelocity.magnitude;
		
		

		// Euler integrate (per frame) velocity into position
		// TODO: Change for a motor
		Profiler.BeginSample("Applying displacement");
		var delta = (newVelocity * elapsedTime);
		if (_characterController != null) 
		{
			_characterController.Move(delta);
		}
		else if (_rigidbody == null || _rigidbody.isKinematic)
		{
			transform.position += delta;
		}
		else
		{
			/*
			 * TODO: This is just a quick test and should not remain, as the behavior is not
			 * consistent to that we obtain when moving the transform.
			 */
			_rigidbody.MovePosition (_rigidbody.position + delta);
		}
		Profiler.EndSample();
		

		// regenerate local space (by default: align vehicle's forward axis with
		// new velocity, but this behavior may be overridden by derived classes.)
		RegenerateLocalSpace (newVelocity);
	}	
	#endregion
}


