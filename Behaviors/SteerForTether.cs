using UnityEngine;
using UnitySteer.Helpers;

/// <summary>
/// Steers a vehicle to keep within a certain range of a point
/// </summary>
public class SteerForTether : Steering
{
	#region Private properties
	[SerializeField]
	float _maximumDistance = 30f;
	[SerializeField]
	Vector3 _tetherPosition;
	#endregion
	
	
	#region Public properties
	public float MaximumDistance {
		get {
			return this._maximumDistance;
		}
		set {
			_maximumDistance = Mathf.Clamp(value, 0, float.MaxValue);
		}
	}

	public Vector3 TetherPosition {
		get {
			return this._tetherPosition;
		}
		set {
			_tetherPosition = value;
		}
	}
	#endregion
	

	
	protected override Vector3 CalculateForce ()
	{
		Vector3 steering = Vector3.zero;
		
		var difference = TetherPosition - Vehicle.Position;
		var distance = difference.magnitude;
		if (distance > _maximumDistance)
		{
			steering = difference - Vehicle.Velocity;
		}
		return steering;
	}
}

