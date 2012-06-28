// #define DEBUG_COMFORT_DISTANCE
using UnityEngine;

/// <summary>
/// Steers a vehicle to remain in cohesion with neighbors
/// </summary>
[AddComponentMenu("UnitySteer/Steer/... for Cohesion")]
public class SteerForCohesion : SteerForNeighbors
{
	float _comfortDistanceSquared;

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
	
	protected override Vector3 CalculateNeighborContribution(Vehicle other)
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

