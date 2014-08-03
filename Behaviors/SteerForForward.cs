using UnityEngine;

namespace UnitySteer.Behaviors
{

/// <summary>
/// Trivial example that simply makes the vehicle move towards its forward vector
/// </summary>
[AddComponentMenu("UnitySteer/Steer/... for Forward")]
public class SteerForForward : Steering
{
	Vector3 _desiredForward = Vector3.zero;

	bool _overrideForward;

	/// <summary>
	/// Desired forward vector. If set to Vector3.zero we will steer toward the transform's forward
	/// </summary>
	public Vector3 DesiredForward 
	{ 
		get {
			return _overrideForward ? _desiredForward : Vehicle.Transform.forward;
		}
		set {
			_desiredForward = value;
			_overrideForward = value != Vector3.zero;
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
		return  _overrideForward ? DesiredForward : Vehicle.Transform.forward;
	}
}

}
