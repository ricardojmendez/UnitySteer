using UnityEngine;
using System.Collections;
using UnitySteer;

/// <summary>
/// Allows an override of the Vehicle's forward vector, based on the last 
/// raw force obtained by the AutonomousVehicle attached to this game object.
/// This is likely to override, and work in conjution with, the vehicle's 
/// RegenerateLocalSpace method, and helps avoid having to subclass the
/// vehicle just to alter its angle.
/// 
/// AutonomousVehicle's implementation of altering its turn only by the forward
/// force applied to it is useful for actual vehicles such as cars, but is
/// falling short when dealing with the nuances of other agents.
/// 
/// Creating as a separate behavior to avoid making drastic alterations to
/// AutonomousVehicle on a point upgrade.
/// </summary>
[RequireComponent(typeof(AutonomousVehicle))]
[AddComponentMenu("UnitySteer/Vehicle/Vehicle Look-At Override")]
public class VehicleLookAtOverride : MonoBehaviour {
	
	AutonomousVehicle _vehicle;
	Vector3 _smoothed = Vector3.zero;
	
	[SerializeField]
	float _smoothRate = 0.1f;
	
	/// <summary>
	/// Acceleration look-at smoothing rate.  The higher the value, the 
	/// jerkier the turns are likely to be.
	/// </summary>
	/// <value>
	/// The smooth rate.
	/// </value>
	public float SmoothRate 
	{
		get 
		{
			return this._smoothRate;
		}
		set 
		{
			_smoothRate = value;
		}
	}
	
	
	// Use this for initialization
	void Awake() 
	{
		_vehicle = GetComponent<AutonomousVehicle>();
		if (_vehicle == null)
		{
			Destroy(this);
			throw new  System.Exception("Missing vehicle, cannot continue");
		}
	}
	
	// Update is called once per frame
	void Update() 
	{
		if (_vehicle.CanMove && _vehicle.LastRawForce != Vector3.zero)
		{
			_smoothed = OpenSteerUtility.blendIntoAccumulator(_smoothRate, _vehicle.LastRawForce, _smoothed);
			transform.LookAt(transform.position + _smoothed);
		}
	}
}
