using UnityEngine;
using UnitySteer.Helpers;

public class SteerForPoint: Steering
{
	bool _reportedArrival = false;
	SteeringEventHandler<Vector3> _onArrival;
	
	/// <summary>
	/// The target point.
	/// </summary>
 	public Vector3 TargetPoint;
	
	public SteeringEventHandler<Vector3> OnArrival {
		get {
			return this._onArrival;
		}
		set {
			_onArrival = value;
		}
	}	
	
	void Awake() {
		if (TargetPoint == Vector3.zero)
			TargetPoint = transform.position;
	}
	
	/// <summary>
	/// Calculates the force to apply to the vehicle to reach a point
	/// </summary>
	/// <returns>
	/// A <see cref="Vector3"/>
	/// </returns>
	protected override Vector3 CalculateForce()
	{
		var force = Vehicle.GetSeekVector(TargetPoint);
		
		// Raise the arrival event
		if (!_reportedArrival && _onArrival != null && force == Vector3.zero) {
			_reportedArrival = true;
			_onArrival(new SteeringEvent<Vector3>(this, "arrived", TargetPoint));
		}
		else
			_reportedArrival = force == Vector3.zero;
		
		return force;
	}
}


