#define TRACE_PATH
using System.Collections;
using UnityEngine;
using UnitySteer;


/// <summary>
/// Steers a vehicle to follow a path
/// </summary>
/// <remarks>
/// Calculates the force by aiming towards the closest point in the path 
/// along the vehicle's heading.  It has side effects such as not considering
/// the whole path on sharp turns.
/// 
/// Much like SteerForPathTrivial, it is meant as a trivial example of how 
/// to do path following using the estimated position returned by 
/// Vector3Pathway.MapPointToPath.
/// </remarks>
[AddComponentMenu("UnitySteer/Steer/... for Path - Predictive Trivial")]
public class SteerForPathPredictiveTrivial : Steering
{
	
	#region Private fields
	[SerializeField]
	float _predictionTime = 1f;
	
	Vector3Pathway _path;
	#endregion
	
	
	#region Public properties
	
	/// <summary>
	/// How far ahead to estimate our position
	/// </summary>
	public float PredictionTime {
		get {
			return this._predictionTime;
		}
		set {
			_predictionTime = value;
		}
	}
	
	/// <summary>
	/// Path to follow
	/// </summary>
	public Vector3Pathway Path {
		get {
			return this._path;
		}
		set {
			_path = value;
		}
	}
	
	#endregion

	/// <summary>
	/// Force to apply to the vehicle
	/// </summary>
	/// <returns>
	/// A <see cref="Vector3"/>
	/// </returns>
	protected override Vector3 CalculateForce()
	{
		Vector3 forceNode = Vector3.zero;
		if (_path != null)
		{
			var tStruct = new PathRelativePosition();
			var futurePosition = Vehicle.PredictFuturePosition(_predictionTime);
			var futurePathPoint = _path.MapPointToPath (futurePosition, ref tStruct);
#if TRACE_PATH
			Debug.DrawLine(Vehicle.Position, futurePosition, Color.red);
			Debug.DrawLine(Vehicle.Position, futurePathPoint, Color.green);
#endif
			
			forceNode = Vehicle.GetSeekVector(futurePathPoint, false);
		}
		return forceNode;
	}
}
