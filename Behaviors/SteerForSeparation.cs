using UnityEngine;

namespace UnitySteer.Behaviors
{

/// <summary>
/// Steers a vehicle to keep separate from neighbors
/// </summary>
[AddComponentMenu("UnitySteer/Steer/... for Separation")]
[RequireComponent(typeof(SteerForNeighborGroup))]
public class SteerForSeparation : SteerForNeighbors
{

	/// <summary>
	/// The comfort distance. Any neighbors closer than this will be hit with an
	/// extra penalty.
	/// </summary>
	[SerializeField]
	float _comfortDistance = 1;

	/// <summary>
	/// How much of a multiplier is applied to neighbors that are inside our
	/// comfort distance.  Defaults to 1 so that we don't change the behavior
	/// of already-created boids.
	/// </summary>
	[SerializeField]
	float _multiplierInsideComfortDistance = 1;

	/// <summary>
	/// The range beyond which we no longer apply any separation force.
	/// </summary>
	[SerializeField]
	float _maximumSeparation = 2f;

	/// <summary>
	/// How much impact the radius of the vehicles involved makes. If 0,
	/// we return a direction vector, otherwise we scale it by the sum
	/// of the radii multiplied by the impact.
	/// </summary>
	[SerializeField]
	float _vehicleRadiusImpact = 2;

	float _comfortDistanceSquared;
	float _maximumSeparationSquared;


    /// <summary>
    /// The comfort distance. Any neighbors closer than this will be hit with an
    /// extra penalty.
    /// </summary>
    public float ComfortDistance
	{
		get { return _comfortDistance; }
		set 
		{ 
			_comfortDistance = value;
			_comfortDistanceSquared = _comfortDistance * _comfortDistance;
		}
	}

    /// <summary>
    /// The range beyond which we no longer apply any separation force.
    /// </summary>
	public float MaximumSeparation
	{
		get { return _maximumSeparation; }
		set
		{
			_maximumSeparation = value;
			_maximumSeparationSquared = value * value;
		}
	}

	protected override void Start()
	{
		_comfortDistanceSquared = _comfortDistance * _comfortDistance;
		_maximumSeparationSquared = _maximumSeparation * _maximumSeparation;
	}


	#region Methods
	public override Vector3 CalculateNeighborContribution(Vehicle other)
	{
		Vector3 steering = Vector3.zero;

		// add in steering contribution
		// (opposite of the offset direction, divided once by distance
		// to normalize, divided another time to get 1/d falloff)
		var offset = other.Position - Vehicle.Position;
		var offsetSqrMag = offset.sqrMagnitude;
		if (offsetSqrMag <= _maximumSeparationSquared)
		{
			steering = (offset / -offsetSqrMag);
			if (!Mathf.Approximately(_multiplierInsideComfortDistance, 1) && offsetSqrMag < _comfortDistanceSquared)
			{
				steering *= _multiplierInsideComfortDistance;
			}

			if (_vehicleRadiusImpact > 0)
			{
				steering *= (other.Radius + Vehicle.Radius) * _vehicleRadiusImpact;
			}

		}
		return steering;
	}
	#endregion
}

}
