using UnityEngine;
using UnitySteer.Helpers;

[AddComponentMenu("UnitySteer/Steer/... to Follow")]
public class SteerToFollow : Steering
{
	
	/// <summary>
	/// Target transform
	/// </summary>
	[SerializeField]
	Transform _target;

	/// <summary>
	/// Should the vehicle's velocity be considered in the seek calculations?
	/// </summary>
	/// <remarks>
	/// If true, the vehicle will slow down as it approaches its target
	/// </remarks>
	[SerializeField]
	bool _considerVelocity = true;
	
	
	/// <summary>
	/// The target.
	/// </summary>
 	public Transform Target
	{
		get { return _target; }
		set
		{
			_target = value;
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

	
	
	public new void Start()
	{
		base.Start();
		
		if (Target == null)
		{
			Target = transform;
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
		return Vehicle.GetSeekVector(Target.position, _considerVelocity);
	}
}


