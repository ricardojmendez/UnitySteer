#define TRACE_ADJUSTMENTS
using UnityEngine;
using UnitySteer;
using System.Linq;
using TickedPriorityQueue;

/// <summary>
/// Vehicle subclass oriented towards autonomous bipeds and vehicles, which 
/// will be ticked automatically to calculate their direction.
/// </summary>
public abstract class TickedVehicle : Vehicle
{
	#region Internal state values
	Vector3 _smoothedAcceleration = Vector3.zero;
	TickedObject _tickedObject;
	UnityTickedQueue _steeringQueue;

	[SerializeField]
	float _accelerationSmoothRate = 0.4f;
	
    /// <summary>
    /// The name of the steering queue for this ticked vehicle.
    /// </summary>
	[SerializeField]
	string _queueName = "Steering";
	
    /// <summary>
    /// How often will this Vehicle's steering calculations be ticked.
    /// </summary>
	[SerializeField]
	float _tickLength = 0.1f;	
    
    /// <summary>
    /// The maximum number of radar update calls processed on the queue per update
    /// </summary>
    /// <remarks>
    /// Notice that this is a limit shared across queue items of the same name, at
    /// least until we have some queue settings, so whatever value is set last for 
    /// the queue will win.  Make sure your settings are consistent for objects of
    /// the same queue.
    /// </remarks>
    [SerializeField]
    int _maxQueueProcessedPerUpdate = 20;

	[SerializeField]
	bool _traceAdjustments = false;	
	#endregion
	
	/// <summary>
	/// Gets or sets the acceleration smooth rate.
	/// </summary>
	/// <value>
	/// The acceleration smooth rate. The higher it is, the more abrupt 
	/// the acceleration is likely to be.  A value of close to 0 causes 
	/// the acceleration to change _very_ slowly. A value of either 0 
	/// or 1 means that any acceleration changes will be directly applied.
	/// </value>
	public float AccelerationSmoothRate 
	{
		get { return this._accelerationSmoothRate; 	}
		set { _accelerationSmoothRate = value; 	}
	}
	

	public CharacterController CharacterController { get; private set; }

	public float LastTickTime { get; private set; }
	
	/// <summary>
	/// Velocity vector used to orient the agent.
	/// </summary>
	/// <remarks>
	/// This is expected to be set by the subclasses.
	/// </remarks>
	public Vector3 OrientationVelocity { get; protected set; }
	
	public string QueueName 
	{
		get { return _queueName; }
		set { _queueName = value; }
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
	public TickedObject TickedObject  { get; private set; }

	
	#region Unity events
	void Start()
	{
		CharacterController = GetComponent<CharacterController>();
		LastTickTime = 0;
	}

	
	protected virtual void OnEnable()
	{
		TickedObject = new TickedObject(OnUpdateSteering);
		TickedObject.TickLength = _tickLength;
		_steeringQueue = UnityTickedQueue.GetInstance(QueueName);
		_steeringQueue.Add(TickedObject);
        _steeringQueue.MaxProcessedPerUpdate = _maxQueueProcessedPerUpdate;
	}
	
	protected virtual void OnDisable()
	{
		if (_steeringQueue != null)
		{
			_steeringQueue.Remove(TickedObject);
		}
	}
	#endregion
	

	#region Velocity / Speed methods
	protected void OnUpdateSteering(object obj)
	{
		// We just calculate the forces, and expect the radar updates
		// itself.
		CalculateForces();
	}



	protected void CalculateForces()
	{
		if (!CanMove || MaxForce == 0 || MaxSpeed == 0)
		{
			return;
		}
		Profiler.BeginSample("Calculating vehicle forces");
		
		var force = Vector3.zero;
		
		Profiler.BeginSample("Adding up basic steerings");
        for(int i = 0; i < Steerings.Length; i++) {
            var s = Steerings[i];
            if (s.enabled) {
                force += s.WeighedForce;
            }
        }
		Profiler.EndSample();
		
		var elapsedTime = Time.time - LastTickTime;
		LastTickTime = Time.time;
		if (IsPlanar)
		{
			force.y = 0;
		}
		LastRawForce = force;
		
		// enforce limit on magnitude of steering force
		Vector3 newAcceleration = Vector3.ClampMagnitude(force / Mass, MaxForce);

		if (newAcceleration.sqrMagnitude == 0)
		{
			ZeroVelocity();
			DesiredVelocity = Vector3.zero;
		}

		var newVelocity = Velocity + newAcceleration * elapsedTime;

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
        for (int i = 0; i < SteeringPostprocessors.Length; i++) {
            var s = SteeringPostprocessors[i];
            if (s.enabled) {
			    adjustedVelocity += s.WeighedForce;
            }
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
		UpdateOrientationVelocity(newVelocity);
		Profiler.EndSample();
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
		var delta = CalculatePositionDelta(elapsedTime);

		/*
			Damp out abrupt changes and oscillations in steering acceleration
			(rate is proportional to time step, and clipped to [0,1])
			
			The higher the smoothRate parameter, the more noise there is
			likely to be in the movement.
		*/
		if (_accelerationSmoothRate > 0)
		{
			_smoothedAcceleration = OpenSteerUtility.blendIntoAccumulator(_accelerationSmoothRate,
			                                                              delta,
			                                                              _smoothedAcceleration);
		}
		else
		{
			_smoothedAcceleration = delta;
		}
		if (CharacterController != null) 
		{
			CharacterController.Move(_smoothedAcceleration);
		}
		else if (Rigidbody == null || Rigidbody.isKinematic)
		{
			Transform.position += _smoothedAcceleration;
		}
		else
		{
			Rigidbody.MovePosition(Rigidbody.position + _smoothedAcceleration);
		}
	}	
	
	
	/// <summary>
	/// Turns the vehicle towards his velocity vector. Previously called
	/// LookTowardsVelocity.
	/// </summary>
	protected void AdjustOrientation(float deltaTime)
	{
		/* 
		 * Avoid adjusting if we aren't applying any velocity. We also
		 * disregard very small velocities, to avoid jittery movement on
		 * rounding errors.
		 */
 		if (DesiredSpeed > MinSpeedForTurning && Velocity != Vector3.zero)
		{
			var newForward = OrientationVelocity;
			if (IsPlanar)
			{
				newForward.y = 0;
			}
			
			if (TurnTime > 0)
			{
				newForward = Vector3.Slerp(Transform.forward, newForward, deltaTime / TurnTime);
			}
			Transform.forward = newForward;
		}
	}

	/// <summary>
	/// Records the velocity that was ust calculated by CalculateForces in a
	/// manner that is specific to each subclass. 
	/// </summary>
	public abstract void UpdateOrientationVelocity(Vector3 velocity);

	/// <summary>
	/// Calculates how much the agent's position should change in a manner that
	/// is specific to the vehicle's implementation.
	/// </summary>
	protected abstract Vector3 CalculatePositionDelta(float deltaTime);

	protected abstract void ZeroVelocity();
	#endregion


	void Update()
	{
		// We still update the forces if the vehicle cannot move, as the
		// calculations on those steering behaviors might be relevant for
		// other methods, but we don't apply it.  
		//
		// If you don't want to have the forces calculated at all, simply
		// disable the vehicle.
		if (CanMove)
		{
			ApplySteeringForce(Time.deltaTime);
			AdjustOrientation(Time.deltaTime);
		}
		else 
		{
			ZeroVelocity();
		}
	}

	[System.Diagnostics.Conditional("TRACE_ADJUSTMENTS")]
	void TraceDisplacement(Vector3 delta, Color color)
	{
		if (_traceAdjustments)
		{
			Debug.DrawLine(Transform.position, Transform.position + delta, color);
		}
	}


}


