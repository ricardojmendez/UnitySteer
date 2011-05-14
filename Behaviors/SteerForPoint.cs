using UnityEngine;
using UnitySteer.Helpers;

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
	Vector3 _targetPoint = Vector3.zero;
	
	
	/// <summary>
	/// The target point.
	/// </summary>
 	public Vector3 TargetPoint
	{
		get { return _targetPoint; }
		set
		{
			_targetPoint = value;
			ReportedArrival = false;
		}
	}

	
	
	public new void Start()
	{
		base.Start();
		
		if (TargetPoint == Vector3.zero)
		{
			TargetPoint = transform.position;
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
		return Vehicle.GetSeekVector(TargetPoint, false);
	}
}


