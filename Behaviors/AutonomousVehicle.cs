using UnityEngine;
using UnitySteer;

/// <summary>
/// Vehicle subclass which automatically applies the steering forces from
/// the components attached to the object.
/// </summary>
[AddComponentMenu("UnitySteer/Vehicle/Autonomous")]
public class AutonomousVehicle : Vehicle
{
	#region Internal state values
	Vector3 _smoothedAcceleration;
	Rigidbody _rigidbody;
	CharacterController _characterController;
	
	[SerializeField]
	float _accelerationSmoothRate = 0.4f;
	
	Vector3 _lastRawForce = Vector3.zero;
	Vector3 _lastAppliedVelocity = Vector3.zero;
	
	#endregion
	
	/// <summary>
	/// Gets or sets the acceleration smooth rate.
	/// </summary>
	/// <value>
	/// The acceleration smooth rate. The higher it is, the more abrupt 
	/// the acceleration is likely to be.
	/// </value>
	public float AccelerationSmoothRate {
		get {
			return this._accelerationSmoothRate;
		}
		set {
			_accelerationSmoothRate = value;
		}
	}
	
	
	public Vector3 LastRawForce {
		get {
			return this._lastRawForce;
		}
	}	
	
	
	public Vector3 LastAppliedVelocity {
		get {
			return this._lastAppliedVelocity;
		}
	}


	#region Methods
	void Start()
	{
		_rigidbody = GetComponent<Rigidbody>();
		_characterController = GetComponent<CharacterController>();
		if (HasInertia)
		{
			Debug.LogError("AutonomousVehicle should not have HasInertia set to TRUE. See the release notes of UnitySteer 2.1 for details.");
		}
	}
		
	
	void FixedUpdate()
	{
		var force = Vector3.zero;
		Profiler.BeginSample("Calculating forces");
		foreach (var steering in Steerings)
		{
			if (steering.enabled)
			{
				force += steering.WeighedForce;
			}
		}

		Profiler.EndSample();
		
		// We still update the forces if the vehicle cannot move, as the
		// calculations on those steering behaviors might be relevant for
		// other methods, but we don't apply it.  
		//
		// If you don't want to have the forces calculated at all, simply
		// disable the vehicle.
		if (CanMove)
		{
			ApplySteeringForce(force, Time.fixedDeltaTime);
		}
		else 
		{
			Speed = 0;
		}
			
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
		
		if (IsPlanar)
		{
			force.y = 0;
		}
		_lastRawForce = force;
		
		// enforce limit on magnitude of steering force
		Vector3 clippedForce = Vector3.ClampMagnitude(force, MaxForce);

		// compute acceleration and velocity
		Vector3 newAcceleration = (clippedForce / Mass);
		
		if (newAcceleration.sqrMagnitude == 0)
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
		if (_accelerationSmoothRate > 0)
		{
			_smoothedAcceleration = OpenSteerUtility.blendIntoAccumulator(_accelerationSmoothRate,
										newAcceleration,
										_smoothedAcceleration);
		}
		else
		{
			_smoothedAcceleration = newAcceleration;
		}

		// Euler integrate (per frame) acceleration into velocity
		newVelocity += _smoothedAcceleration * elapsedTime;

		// enforce speed limit
		newVelocity = Vector3.ClampMagnitude(newVelocity, MaxSpeed);

		// update Speed
		_lastAppliedVelocity = newVelocity;
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
			_rigidbody.MovePosition (_rigidbody.position + delta);
		}
		Profiler.EndSample();
		

		// regenerate local space (by default: align vehicle's forward axis with
		// new velocity, but this behavior may be overridden by derived classes.)
		LookTowardsVelocity (_lastAppliedVelocity, elapsedTime);
	}
	#endregion
}


