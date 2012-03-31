#define TRACE_ADJUSTMENTS
using UnityEngine;
using UnitySteer;
using System.Linq;
using TickedPriorityQueue;

/// <summary>
/// Vehicle subclass oriented towards autonomous bipeds, which have a movement
/// vector which can be separate from their forward vector.   
/// </summary>
/// <remarks>
/// This class currently contains duplicated code from AutonomousVehicle, since
/// I have just split them, something that I will refactor before merging into
/// master.
/// </remarks>
[AddComponentMenu("UnitySteer/Vehicle/Biped")]
public class Biped : Vehicle
{
	#region Internal state values
	Vector3 _smoothedAcceleration;
	Rigidbody _rigidbody;
	CharacterController _characterController;
	TickedObject _tickedObject;
	UnityTickedQueue _steeringQueue;
	float _lastTickTime;

	/// <summary>
	/// The magnitude of the last velocity vector assigned to the vehicle 
	/// </summary>
	float _speed = 0;

	/// <summary>
	/// The biped's current velocity vector
	/// </summary>
	Vector3 _velocity;


	
	[SerializeField]
	string _queueName = "Steering";
	
	[SerializeField]
	float _tickLength = 0.1f;	
	
		
	[SerializeField]
	bool _traceAdjustments = false;
	
	[SerializeField]
	float _accelerationSmoothRate = 0.4f;	
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
	
	public string QueueName 
	{
		get { return _queueName; }
		set { _queueName = value; }
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
	/// Priority queue for this vehicle's updates
	/// </summary>
	public UnityTickedQueue SteeringQueue 
	{
		get { return this._steeringQueue; }
	}


	/// <summary>
	/// Ticked object for the vehicle, so that its owner can configure
	/// the priority as desired.
	/// </summary>
	public TickedObject TickedObject {
		get {
			return this._tickedObject;
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
			NormalizedVelocity = _speed != 0 ? _velocity / _speed : Vector3.zero;
		}
	}

	/// <summary>
	/// Normalized velocity vector of the vehicle
	/// </summary>
	public Vector3 NormalizedVelocity
	{
		get; protected set;
	}

	
	#region Methods
	void Start()
	{
		_rigidbody = GetComponent<Rigidbody>();
		_characterController = GetComponent<CharacterController>();
		_lastTickTime = 0;
	}

	
	void OnEnable()
	{
		_tickedObject = new TickedObject(OnUpdateSteering);
		_tickedObject.TickLength = _tickLength;
		_steeringQueue = UnityTickedQueue.GetInstance(QueueName);
		_steeringQueue.Add(_tickedObject);
		Velocity = Vector3.zero;
	}
	
	void OnDisable()
	{
		if (_steeringQueue != null)
		{
			_steeringQueue.Remove(_tickedObject);
		}
	}
	
	protected void OnUpdateSteering(object obj)
	{
		// We just calculate the forces, and expect the radar updates
		// itself.
		CalculateForces();
	}
	
	
	void CalculateForces()
	{
		if (!CanMove || MaxForce == 0 || MaxSpeed == 0)
		{
			return;
		}
		Profiler.BeginSample("Calculating vehicle forces");
		
		var force = Vector3.zero;
		
		Profiler.BeginSample("Adding up basic steerings");
		foreach(var s in Steerings.Where( s => s.enabled ))
		{
			force += s.WeighedForce;
		}
		Profiler.EndSample();
		
		var elapsedTime = Time.time - _lastTickTime;
		_lastTickTime = Time.time;
		if (IsPlanar)
		{
			force.y = 0;
		}
		LastRawForce = force;
		
		// enforce limit on magnitude of steering force
		Vector3 clippedForce = Vector3.ClampMagnitude(force, MaxForce);

		// compute acceleration and velocity
		Vector3 newAcceleration = (clippedForce / Mass);
		
		if (newAcceleration.sqrMagnitude == 0)
		{
			Velocity = Vector3.zero;
			DesiredVelocity = Vector3.zero;
		}

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
		
		// Euler integrate (per call time) acceleration into velocity
		var newVelocity = Velocity + _smoothedAcceleration * elapsedTime;

		// Enforce speed limit
		newVelocity = Vector3.ClampMagnitude(newAcceleration, MaxSpeed);
		DesiredVelocity = newVelocity;
		
		// Adjusts the velocity by applying the post-processing behaviors.
		//
		// This currently is not also considering the maximum force, nor 
		// blending the new velocity into an accumulator. We *could* do that,
		// but things are working just fine for now, and it seems like
		// overkill. 
		Vector3 adjustedVelocity = Vector3.zero;
		Profiler.BeginSample("Adding up post-processing steerings");
		foreach (var s in SteeringPostprocessors.Where( s => s.enabled ))
		{
			adjustedVelocity += s.WeighedForce;
		}
		Profiler.EndSample();
		if (adjustedVelocity != Vector3.zero)
		{
			adjustedVelocity = Vector3.ClampMagnitude(adjustedVelocity, MaxSpeed);
			TraceDisplacement(adjustedVelocity, Color.cyan);
			TraceDisplacement(newVelocity, Color.white);
			newVelocity = adjustedVelocity;
		}
		
		// Update vehicle velocity
		Velocity = newVelocity;
		Profiler.EndSample();
	}
	
	
	void FixedUpdate()
	{
		// We still update the forces if the vehicle cannot move, as the
		// calculations on those steering behaviors might be relevant for
		// other methods, but we don't apply it.  
		//
		// If you don't want to have the forces calculated at all, simply
		// disable the vehicle.
		if (CanMove)
		{
			ApplySteeringForce(Time.fixedDeltaTime);
			LookTowardsVelocity(Time.fixedDeltaTime);
		}
		else 
		{
			Velocity = Vector3.zero;
		}
	}


	void LookTowardsVelocity (float elapsedTime)
	{
		/* 
		 * Avoid adjusting if we aren't applying any velocity. We also
		 * disregard very small velocities, to avoid jittery movement on
		 * rounding errors.
		 */
 		if (Speed > MinSpeedForTurning && Velocity != Vector3.zero)
		{
			var newForward = NormalizedVelocity;
			if (IsPlanar)
			{
				newForward.y = 0;
				newForward.Normalize();
			}
			
			if (TurnTime != 0)
			{
				newForward = Vector3.Slerp(_transform.forward, newForward, elapsedTime / TurnTime);
			}
			_transform.forward = newForward;
		}
	}

	
	/// <summary>
	/// Applies a steering force to this vehicle
	/// </summary>
	/// <param name="elapsedTime">
	/// How long has elapsed since the last update<see cref="System.Single"/>
	/// </param>
	void ApplySteeringForce(float elapsedTime)
	{
		// Euler integrate (per frame) velocity into position
		var delta = (Velocity * elapsedTime);
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
	}
	#endregion
	
	[System.Diagnostics.Conditional("TRACE_ADJUSTMENTS")]
	void TraceDisplacement(Vector3 delta, Color color)
	{
		if (_traceAdjustments)
		{
			Debug.DrawLine(transform.position, transform.position + delta, color);
		}
	}
}


