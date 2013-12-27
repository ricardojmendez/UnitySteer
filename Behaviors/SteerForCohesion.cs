// #define DEBUG_COMFORT_DISTANCE
using UnityEngine;

namespace UnitySteer.Base
{

/// <summary>
/// Steers a vehicle to remain in cohesion with neighbors
/// </summary>
[AddComponentMenu("UnitySteer/Steer/... for Cohesion")]
[RequireComponent(typeof(SteerForNeighborGroup))]
public class SteerForCohesion : SteerForNeighbors
{
	float _comfortDistanceSquared;

	/// <summary>
	/// Any neighbor that is closer than the comfort distance will not be
	/// considered for cohesion.
	/// </summary>
	[SerializeField]
	float _comfortDistance = 1;

	public float ComfortDistance
	{
		get { return _comfortDistance; }
		set 
		{ 
			_comfortDistance = value;
			_comfortDistanceSquared = _comfortDistance * _comfortDistance;
		}
	}

	protected override void Start()
	{
		_comfortDistanceSquared = _comfortDistance * _comfortDistance;
	}
	
	public override Vector3 CalculateNeighborContribution(Vehicle other)
	{
		// accumulate sum of forces leading us towards neighbor's positions
		var distance = other.Position - Vehicle.Position;
		if (distance.sqrMagnitude < _comfortDistanceSquared)
		{
			distance = Vector3.zero;
		}
		return distance;
	}
	
	void OnDrawGizmos() {
		#if DEBUG_COMFORT_DISTANCE
		Gizmos.color = Color.magenta;
		Gizmos.DrawWireSphere(transform.position, ComfortDistance);
		#endif		
	}
}

}
