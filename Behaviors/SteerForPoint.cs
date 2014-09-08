using UnityEngine;

namespace UnitySteer.Behaviors
{

[AddComponentMenu("UnitySteer/Steer/... for Point")]
public class SteerForPoint : Steering
{
	
	/// <summary>
	/// Target point
	/// </summary>
	/// <remarks>
	/// Declared as a separate value so that we can inspect it on Unity in 
	/// debug mode.
	/// </remarks>
	[SerializeField] private Vector3 _targetPoint = Vector3.zero;

	/// <summary>
	/// Should the vehicle's velocity be considered in the seek calculations?
	/// </summary>
	/// <remarks>
	/// If true, the vehicle will slow down as it approaches its target. See
	/// the remarks on GetSeekVector.
	/// </remarks>
	[SerializeField] private bool _considerVelocity;

	/// <summary>
	/// Should the target default to the vehicle current position if it's set to Vector3.zero?
	/// </summary>
	[SerializeField] private bool _defaultToCurrentPosition = true;
	
	
	/// <summary>
	/// The target point.
	/// </summary>
 	public Vector3 TargetPoint
	{
		get { return _targetPoint; }
		set
		{
		    if (_targetPoint == value) return;
			_targetPoint = value;
			ReportedArrival = false;
		}
	}


	/// <summary>
	/// Should the vehicle's velocity be considered in the seek calculations?
	/// </summary>
	/// <remarks>
	/// If true, the vehicle will slow down as it approaches its target
	/// </remarks>
 	public bool ConsiderVelocity
	{
		get { return _considerVelocity; }
		set { _considerVelocity = value; }
	}
	
	protected override void Start()
	{
		base.Start();
		
		if (_defaultToCurrentPosition && TargetPoint == Vector3.zero)
		{
			enabled = false;
		}
	}
	
	/// <summary>
	/// Calculates the force to apply to the vehicle to reach a point
	/// </summary>
	/// <returns>
	/// A <see cref="Vector3"/>
	/// </returns>
	protected override Vector3 CalculateForce()
	{
		return Vehicle.GetSeekVector(TargetPoint, _considerVelocity);
	}
}

}
