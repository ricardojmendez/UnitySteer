using UnityEngine;
using UnitySteer;
using System.Linq;


/// <summary>
/// Base class for vehicles. It does not move the objects, and instead 
/// provides a set of basic functionality for its subclasses.  See
/// AutonomousVehicle for one that does apply the steering forces.
/// </summary>
/// <remarks>The main reasoning behind having a base vehicle class that is not
/// autonomous in a library geared towards autonomous vehicles, is that in
/// some circumstances we want to treat agents such as the player (wihch is not
/// controlled by our automated steering functions) the same as other 
/// vehicles, at least for purposes of estimation, avoidance, pursuit, etc.
/// In this case, the base Vehicle class can be used to provide an interface
/// to whatever is doing the moving, like a CharacterMotor.</remarks>
[AddComponentMenu("UnitySteer/Vehicle/Vehicle")]
public abstract class Vehicle : DetectableObject
{	
	[SerializeField]
	float _minSpeedForTurning = 0.1f;
	
	[SerializeField]
	int _movementPriority = 0;	
	
	#region Private fields
	float _squaredArrivalRadius;
	
	[SerializeField]
	float _turnTime = 0.25f;
	
	[SerializeField]
	/// <summary>
	/// Vehicle's mass
	/// </summary>
	/// <remarks>
	/// This value will be used when applying forces to the vehicle.
	/// <remarks>
	float _mass = 1;
	
	[SerializeField]
	bool _isPlanar = false;
	
	/// <summary>
	/// The vehicle's arrival radius.
	/// </summary>
	/// <remarks>The difference between the radius and arrival radius is that
	/// the first is used to determine the area the vehicle covers, whereas the
	/// second one is a value used to determine if a vehicle is close enough
	/// to a desired target.  Unlike the radius, it is not scaled with the vehicle.</remarks>
	[SerializeField]
	float _arrivalRadius = 1;	
	
	
	[SerializeField]
	float _maxSpeed = 1;

	[SerializeField]
	float _maxForce = 10;

	/// <summary>
	/// Indicates if the behavior should move or not
	/// </summary>
	[SerializeField]
	bool _canMove = true;
	
	#endregion


	#region Public properties
	/// <summary>
	/// Indicates if the current vehicle can move
	/// </summary>
	public bool CanMove 
	{
		get { return _canMove; }
		set { _canMove = value; }
	}
	
	/// <summary>
	/// The velocity desired by this vehicle, likely calculated by means 
	/// similar to what AutonomousVehicle does
	/// </summary>
	public Vector3 DesiredVelocity { get; protected set; }
    
    public GameObject GameObject { get; private set; }
	
	
	/// <summary>
	/// Does the vehicle move in Y space?
	/// </summary>
	public bool IsPlanar 
	{
		get { return this._isPlanar; }
		set { _isPlanar = value; }
	}

	/// <summary>
	/// Mass for the vehicle
	/// </summary>
	public float Mass {
		get { return _mass; }
		set { _mass = Mathf.Max(0, value); }
	}

	/// <summary>
	/// Maximum force that can be applied to the vehicle
	/// </summary>
	public float MaxForce {
		get {
			return this._maxForce;
		}
		set {
			_maxForce = Mathf.Clamp(value, 0, float.MaxValue);
		}
	}

	/// <summary>
	/// The vehicle's maximum speed
	/// </summary>
	public float MaxSpeed {
		get {
			return this._maxSpeed;
		}
		set {
			_maxSpeed = Mathf.Clamp(value, 0, float.MaxValue);
		}
	}
	
	public int MovementPriority 
	{
		get { return _movementPriority; }
	}
	
	public float MinSpeedForTurning
	{
		get { return _minSpeedForTurning; }
	}


	/// <summary>
	/// Radar assigned to this vehicle
	/// </summary>
	public Radar Radar 
	{
		get; private set;
	}
	
    public Rigidbody Rigidbody { get; private set; }
    
	/// <summary>
	/// Speedometer attached to the same object as this vehicle, if any
	/// </summary>
	public Speedometer Speedometer  { get; protected set; }


	/// <summary>
	/// Vehicle arrival radius
	/// </summary>
	public float ArrivalRadius {
		get { return _arrivalRadius; }
		set 
		{
			_arrivalRadius = Mathf.Clamp(value, 0.01f, float.MaxValue);
			RecalculateScaledValues();			
		}
	}
	
	public float SquaredArrivalRadius 
	{
		get { return this._squaredArrivalRadius; }
	}

	/// <summary>
	/// Last raw force applied to the vehicle. It is expected to be set 
	/// by the subclases.
	/// </summary>
	public Vector3 LastRawForce { get; protected set; }

	/// <summary>
	/// Current vehicle speed
	/// </summary>
	public abstract float Speed { get; set; }
	
	/// <summary>
	/// How quickly does the vehicle turn toward a vector.
	/// </summary>
	/// <value>
	/// The turn speed
	/// </value>
	public float TurnTime 
	{
		get 
		{
			return _turnTime;
		}
		set 
		{
			_turnTime = Mathf.Max(0, value);
		}
	}

	/// <summary>
	/// Array of steering behaviors
	/// </summary>
	public Steering[] Steerings { get; private set; }
	
	/// <summary>
	/// Array of steering post-processor behaviors
	/// </summary>
	public Steering[] SteeringPostprocessors { get; private set; }

	/// <summary>
	/// Current vehicle velocity. Subclasses are likely to only actually
	/// implement one of the two methods.
	/// </summary>
	public abstract Vector3 Velocity { get; set; }

	/// <summary>
	/// Current magnitude for the vehicle's velocity.
	/// </summary>
	/// <remarks>
	/// It is expected to be set at the same time that the Velocity is 
	/// assigned in one of the descendent classes.  It may or may not
	/// match the vehicle speed, depending on how that is calculated - 
	/// for example, some subclasses can use a Speedometer to calculate
	/// their speed.
	/// </remarks>
	public float DesiredSpeed { get; protected set; }
	
	#endregion

	#region Unity methods
	protected override void Awake()
	{
		base.Awake();
        GameObject = gameObject;
        Rigidbody = GetComponent<Rigidbody>();  
		var allSteerings = GetComponents<Steering>();
		Steerings = allSteerings.Where( x => !x.IsPostProcess ).ToArray();
		SteeringPostprocessors = allSteerings.Where( x => x.IsPostProcess ).ToArray();
		
		if (_movementPriority == 0)
		{
			_movementPriority = gameObject.GetInstanceID();
		}
		Radar = this.GetComponent<Radar>();
		Speedometer = this.GetComponent<Speedometer>();
	}
	#endregion
	
	
	#region Methods	
	/// <summary>
	/// Adjust the steering force passed to ApplySteeringForce.
	/// </summary>
	/// <param name="force">
	/// A force to be applied to the vehicle<see cref="Vector3"/>
	/// </param>
	/// <returns>
	/// Adjusted force vector <see cref="Vector3"/>
	/// </returns>
	/// <remarks>
	/// Allows a specific vehicle class to redefine this adjustment.
	/// The default will return the value unmodified.
	/// </remarks>
	protected virtual Vector3 AdjustRawSteeringForce(Vector3 force)
	{
		return force;
	}

	/// <summary>
	/// Recalculates the vehicle's scaled radius and center
	/// </summary>
	protected override void RecalculateScaledValues() 
	{
		base.RecalculateScaledValues();
		_squaredArrivalRadius = _arrivalRadius * _arrivalRadius;
	}
	
	
	/// <summary>
	/// Predicts where the vehicle will be at a point in the future
	/// </summary>
	/// <param name="predictionTime">
	/// A time in seconds for the prediction <see cref="System.Single"/>
	/// </param>
	/// <returns>
	/// Vehicle position<see cref="Vector3"/>
	/// </returns>
	public override Vector3 PredictFuturePosition(float predictionTime)
    {
        return Transform.position + (Velocity * predictionTime);
	}

	/// <summary>
	/// Predicts where the vehicle wants to be at a point in the future
	/// </summary>
	/// <param name="predictionTime">
	/// A time in seconds for the prediction <see cref="System.Single"/>
	/// </param>
	/// <returns>
	/// Vehicle position<see cref="Vector3"/>
	/// </returns>
	public Vector3 PredictFutureDesiredPosition(float predictionTime)
	{
		return Transform.position + (DesiredVelocity * predictionTime);
	}
	
	
	/// <summary>
	/// Calculates if a vehicle is in the neighborhood of another
	/// </summary>
	/// <param name="other">
	/// Another vehicle to check against<see cref="Vehicle"/>
	/// </param>
	/// <param name="minDistance">
	/// Minimum distance <see cref="System.Single"/>
	/// </param>
	/// <param name="maxDistance">
	/// Maximum distance <see cref="System.Single"/>
	/// </param>
	/// <param name="cosMaxAngle">
	/// Cosine of the maximum angle between vehicles (for performance)<see cref="System.Single"/>
	/// </param>
	/// <returns>
	/// True if within the neighborhood, or false if otherwise<see cref="System.Boolean"/>
	/// </returns>
	/// <remarks>Originally SteerLibrary.inBoidNeighborhood</remarks>
	public bool IsInNeighborhood (Vehicle other, float minDistance, float maxDistance, float cosMaxAngle)
	{
		bool result = false;
		if (other != this)
		{
			Vector3 offset = other.Position - Position;
			float distanceSquared = offset.sqrMagnitude;

			// definitely in neighborhood if inside minDistance sphere
			if (distanceSquared < (minDistance * minDistance))
			{
				result = true;
			}
			else
			{
				// definitely not in neighborhood if outside maxDistance sphere
				if (distanceSquared > (maxDistance * maxDistance))
				{
					result = false;
				}
				else
				{
					// otherwise, test angular offset from forward axis
					Vector3 unitOffset = offset / (float) Mathf.Sqrt (distanceSquared);
					float forwardness = Vector3.Dot(Transform.forward, unitOffset);
					result = forwardness > cosMaxAngle;
				}
			}
		}
		return result;
	}
		
	
	/// <summary>
	/// Returns a vector to seek a target position
	/// </summary>
	/// <param name="target">
	/// Target position <see cref="Vector3"/>
	/// </param>
	/// <param name="considerVelocity">
	/// Should the current velocity be taken into account?
	/// </param>
	/// <returns>
	/// Seek vector <see cref="Vector3"/>
	/// </returns>
	public Vector3 GetSeekVector(Vector3 target, bool considerVelocity = true)
	{
		/*
		 * First off, we calculate how far we are from the target, If this
		 * distance is smaller than the configured vehicle radius, we tell
		 * the vehicle to stop.
		 */
		Vector3 force = Vector3.zero;
		
		// If we're dealing with a planar vehicle, disregard the target's 
		// Y position from the calculation
		if (IsPlanar)
		{
			target.y = Position.y;
		}
		
		var difference = target - Position;
        float d = difference.sqrMagnitude;
        if (d > SquaredArrivalRadius)
		{
			/*
			 * But suppose we still have some distance to go. The first step
			 * then would be calculating the steering force necessary to orient
			 * ourselves to and walk to that point.
			 * 
			 * It doesn't apply the steering itself, simply returns the value so
			 * we can continue operating on it.
			 */
			force = considerVelocity ?  difference - Velocity : difference;
		}
		return force;
	}
	
	/// <summary>
	/// Returns a returns a maxForce-clipped steering force along the 
	/// forward vector that can be used to try to maintain a target speed
	/// </summary>
	/// <returns>
	/// The target speed vector.
	/// </returns>
	/// <param name='targetSpeed'>
	/// Target speed to aim for.
	/// </param>
	public Vector3 GetTargetSpeedVector(float targetSpeed) {
		 float mf = MaxForce;
		 float speedError = targetSpeed - Speed;
		 return Transform.forward * Mathf.Clamp (speedError, -mf, +mf);		
	}
	
	
	/// <summary>
	/// Returns the distance from the this vehicle to another
	/// </summary>
	/// <returns>
	/// The distance between both vehicles' positions. If negative, they are overlapping.
	/// </returns>
	/// <param name='other'>
	/// Vehicle to compare against.
	/// </param>
	public float DistanceFromPerimeter(Vehicle other) {
		var diff  = Position - other.Position;
		return diff.magnitude - ScaledRadius - other.ScaledRadius;
	}
	
	/// <summary>
	/// Resets the vehicle's orientation.
	/// </summary>
	public void ResetOrientation()
	{
		Transform.up = Vector3.up;
		Transform.forward = Vector3.forward;
	}
	
	public float PredictNearestApproachTime(Vehicle other)
	{
		return PredictNearestApproachTime(other, Velocity);
	}
	
	
    /// <summary>
    /// Predicts the time until nearest approach between this and another vehicle
    /// </summary>
    /// <returns>
    /// The nearest approach time.
    /// </returns>
    /// <param name='other'>
    /// Other vehicle to compare against
    /// </param>
	public float PredictNearestApproachTime (Vehicle other, Vector3 myVelocity)
	{
		// imagine we are at the origin with no velocity,
		// compute the relative velocity of the other vehicle
		Vector3 otherVelocity = other.Velocity;
		Vector3 relVelocity = otherVelocity - myVelocity;
		float relSpeed = relVelocity.magnitude;

		// for parallel paths, the vehicles will always be at the same distance,
		// so return 0 (aka "now") since "there is no time like the present"
		if (relSpeed == 0) return 0;

		// Now consider the path of the other vehicle in this relative
		// space, a line defined by the relative position and velocity.
		// The distance from the origin (our vehicle) to that line is
		// the nearest approach.

		// Take the unit tangent along the other vehicle's path
		Vector3 relTangent = relVelocity / relSpeed;

		// find distance from its path to origin (compute offset from
		// other to us, find length of projection onto path)
		Vector3 relPosition = Position - other.Position;
		float projection = Vector3.Dot(relTangent, relPosition);

		return projection / relSpeed;
	}
	
	
	/// <summary>
	/// Given the time until nearest approach (predictNearestApproachTime)
	/// determine position of each vehicle at that time, and the distance
	/// between them
	/// </summary>
	/// <returns>
	/// Distance between positions
	/// </returns>
	/// <param name='other'>
	/// Other vehicle to compare against
	/// </param>
	/// <param name='time'>
	/// Time to estimate.
	/// </param>
	/// <param name='ourPosition'>
	/// Our position.
	/// </param>
	/// <param name='hisPosition'>
	/// The other vehicle's position.
	/// </param>
	public float ComputeNearestApproachPositions(Vehicle other, float time, 
												  ref Vector3 ourPosition, 
												  ref Vector3 hisPosition)
	{
		return ComputeNearestApproachPositions(other, time, ref ourPosition, ref hisPosition, Speed, Transform.forward);
	}		
	
	/// <summary>
	/// Given the time until nearest approach (predictNearestApproachTime)
	/// determine position of each vehicle at that time, and the distance
	/// between them
	/// </summary>
	/// <returns>
	/// Distance between positions
	/// </returns>
	/// <param name='other'>
	/// Other vehicle to compare against
	/// </param>
	/// <param name='time'>
	/// Time to estimate.
	/// </param>
	/// <param name='ourPosition'>
	/// Our position.
	/// </param>
	/// <param name='hisPosition'>
	/// The other vehicle's position.
	/// </param>
	/// <param name='ourForward'>
	/// Forward vector to use instead of the vehicle's.
	/// </param>
	public float ComputeNearestApproachPositions(Vehicle other, float time, 
												  ref Vector3 ourPosition, 
												  ref Vector3 hisPosition,
	                                             float ourSpeed,
	                                             Vector3 ourForward)
	{
		Vector3	   myTravel = ourForward 	   		   *	ourSpeed * time;
		Vector3 otherTravel = other.Transform.forward * other.Speed * time;

		ourPosition = Position 		 + myTravel;
		hisPosition = other.Position + otherTravel;

		return Vector3.Distance(ourPosition, hisPosition);
	}	
	
	
	protected override void OnDrawGizmos()
	{
		if (_drawGizmos)
		{
			base.OnDrawGizmos();
			Gizmos.color = Color.grey;
			Gizmos.DrawWireSphere(Position, _arrivalRadius);
		}
	}
	#endregion
}