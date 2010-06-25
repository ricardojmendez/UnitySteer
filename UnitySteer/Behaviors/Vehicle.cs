using UnityEngine;
using UnitySteer;
using System.Collections;


public class Vehicle: MonoBehaviour
{
	#region Internal state values
	float _curvature;
	Vector3 _lastForward;
	Vector3 _lastPosition;
	Vector3 _smoothedAcceleration;
	float _smoothedCurvature;
	Vector3 _smoothedPosition;
	#endregion

	#region Private fields
	Steering[] _steerings;

	[SerializeField]
	private bool _hasInertia = false;

	[SerializeField]
	/// <summary>
	/// Internally-assigned Mass for the vehicle.
	/// </summary>
	/// <remarks>
	/// This value will be disregarded if the object has a rigidbody, and
	/// that rigidbody's mass value will be used instead.
	////remarks>
	private float _internalMass = 0;

	[SerializeField]
	float _radius = 1;

	[SerializeField]
	float _speed = 0;

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
	public bool CanMove {
		get {
			return this._canMove;
		}
		set {
			_canMove = value;
		}
	}

	/// <summary>
	/// Does the vehicle continue going when there's no force applied to it?
	/// </summary>
	public bool HasInertia {
		get {
			return this._hasInertia;
		}
		set {
			_hasInertia = value;
		}
	}

	/// <summary>
	/// Mass for the vehicle
	/// </summary>
	/// <remarks>If the vehicle has a rigidbody, its mass will be updated if
	/// this property is set.</remarks>
	public float Mass {
		get
		{
			return (rigidbody != null) ? rigidbody.mass : _internalMass;
		}
		set
		{
			if( rigidbody != null )
			{
				rigidbody.mass = value;
			}
			else
			{
				_internalMass = value;
			}
		}
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


	/// <summary>
	/// Vehicle radius
	/// </summary>
	public float Radius {
		get {
			return _radius;
		}
		set {
			_radius = Mathf.Clamp(value, 0.01f, float.MaxValue);
		}
	}

	/// <summary>
	/// Current vehicle speed
	/// </summary>
	public float Speed {
		get {
			return _speed;
		}
		set {
			_speed = Mathf.Clamp(value, 0, MaxSpeed);
		}
	}

	/// <summary>
	/// Current vehicle velocity
	/// </summary>
	public Vector3 Velocity
	{
		get
		{
			return transform.forward * _speed;
		}
	}
	#endregion

	#region Methods
	void Start()
	{
		_steerings = this.GetComponents<Steering>();
	}

	void FixedUpdate()
	{
		foreach (var steering in _steerings)
		{
			var force  = steering.Force;
			ApplySteeringForce(force, Time.fixedDeltaTime);
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
		if (MaxForce == 0 || MaxSpeed == 0)
		{
			return;
		}

		force = AdjustRawSteeringForce(force);

		// enforce limit on magnitude of steering force
		Vector3 clippedForce = OpenSteerUtility.truncateLength(force, MaxForce);

		// compute acceleration and velocity
		Vector3 newAcceleration = (clippedForce / Mass);

		if (newAcceleration.sqrMagnitude == 0 && !HasInertia)
		{
			Speed = 0;
		}

		Vector3 newVelocity = Velocity;

		// damp out abrupt changes and oscillations in steering acceleration
		// (rate is proportional to time step, then clipped into useful range)
		#if CLIPPING_ELAPSEDTIME
		/*
			The lower the smoothRate parameter, the more noise there is
			likely to be in the movement.
		 */
		if (elapsedTime > 0)
		{
			/*
				RJM: The clipping of smoothRate is framerate-dependent, which
				is bad when we're trying to get consistent behavior across a
				variety of processor usages.
			 */
			float smoothRate = Mathf.Clamp(9 * elapsedTime, 0.15f, 0.4f);
			_smoothedAcceleration = OpenSteerUtility.blendIntoAccumulator(smoothRate,
										newAcceleration,
										_smoothedAcceleration);
		}
		#else
		_smoothedAcceleration = OpenSteerUtility.blendIntoAccumulator(0.4f,
									newAcceleration,
									_smoothedAcceleration);
		#endif

		// Euler integrate (per frame) acceleration into velocity
		newVelocity += _smoothedAcceleration * elapsedTime;

		// enforce speed limit
		newVelocity = OpenSteerUtility.truncateLength(newVelocity, MaxSpeed);

		// update Speed
		Speed = newVelocity.magnitude;

		// Euler integrate (per frame) velocity into position
		// TODO: Change for a motor
		transform.position += (newVelocity * elapsedTime);

		// regenerate local space (by default: align vehicle's forward axis with
		// new velocity, but this behavior may be overridden by derived classes.)
		RegenerateLocalSpace (newVelocity);

		// maintain path curvature information
		MeasurePathCurvature (elapsedTime);

		// running average of recent positions
		_smoothedPosition = OpenSteerUtility.blendIntoAccumulator(elapsedTime * 0.06f,
								transform.position,
								_smoothedPosition);
	}


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
	/// default is to disallow backward-facing steering at low speed.
	///
	/// xxx should the default be this ad-hocery, or no adjustment?
	/// xxx experimental 8-20-02
	///
	/// NOTE RJM: Upon profiling, this seems to be about 25% of what
	/// applySteeringForce is doing. It might be worth reviewing if it
	/// should be kept around, considering that CWR was unsure if this
	/// "ad-hocery" should remain as default.
	///
	/// This sort of adjustment is definitely useful for vehicles such
	/// as the lightning chain, but might not need to be on the base
	/// class.
	/// </remarks>
	public virtual Vector3 AdjustRawSteeringForce(Vector3 force)
	{
		// Do force adjustment only if the speed is a fifth of our
		// maximum valid speed
		float maxAdjustedSpeed = 0.2f * MaxSpeed;

		if ((Speed > maxAdjustedSpeed) || (force == Vector3.zero))
		{
			return force;
		}
		else
		{
			float range = Speed / maxAdjustedSpeed;
			float cosine = Mathf.Lerp(1.0f, -1.0f, Mathf.Pow(range, 20));
			Vector3 angle = OpenSteerUtility.limitMaxDeviationAngle(force, cosine, transform.forward);
			return angle;
		}
	}

	void RegenerateLocalSpace (Vector3 newVelocity)
	{
		// TODO: We should apply maximum turning speed here.
		// adjust orthonormal basis vectors to be aligned with new velocity
		if (Speed > 0)
			transform.forward = newVelocity / Speed;
	}


	void MeasurePathCurvature (float elapsedTime)
	{
		if (elapsedTime > 0)
		{
			Vector3 pos = transform.position;
			Vector3 fwd = transform.forward;
			Vector3 dP = _lastPosition - pos;
			Vector3 dF = (_lastForward - fwd) / dP.magnitude;
			//SI - BIT OF A WEIRD FIX HERE . NOT SURE IF ITS CORRECT
			//Vector3 lateral = dF.perpendicularComponent (forward ());
			Vector3 lateral = OpenSteerUtility.perpendicularComponent(dF, fwd);

			float sign = (Vector3.Dot(lateral, transform.right) < 0) ? 1.0f : -1.0f;
			_curvature = lateral.magnitude * sign;
			/*
				If elapsedTime is greater than 0.25, that means that blendIntoAccumulator
				will end up clipping the first parameter to [0..1], and we'll lose information,
				making this call framerate-dependent.

				No idea where that 4.0f value comes from, probably out of a hat.
			 */
			_smoothedCurvature = OpenSteerUtility.blendIntoAccumulator(elapsedTime * 4.0f,
									_curvature,
									_smoothedCurvature);

			_lastForward = fwd;
			_lastPosition = pos;
		}
	}
	#endregion
}