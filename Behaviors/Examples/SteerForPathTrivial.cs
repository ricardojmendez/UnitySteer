using System.Collections;
using UnityEngine;
using UnitySteer;


/// <summary>
/// Steers a vehicle to follow a path
/// </summary>
/// <remarks>
/// Trivial path following class which will simply steer for each point 
/// in Vector3Pathway. It is meant as an example of how to start integrating
/// path following with your agents.
/// 
/// Movement *will* look robotic due to its simplicity.
/// </remarks>
[AddComponentMenu("UnitySteer/Steer/... for Path - Trivial")]
public class SteerForPathTrivial : Steering
{
	
	#region Private fields
	
	[SerializeField]
	bool _startsOnFirstSegment = true;
	
	int _currentNode = -1;
	Vector3Pathway _path;
	#endregion
	
	
	#region Public properties
	/// <summary>
	/// Path to follow
	/// </summary>
	public Vector3Pathway Path 
	{
		get {
			return this._path;
		}
		set {
			_path = value;
			_currentNode = FindStartNode();
		}
	}
	
	/// <summary>
	/// Indicates if we start following the path on the first segment, or on the closest one
	/// </summary>
	public bool StartsOnFirstSegment 
	{
		get {
			return this._startsOnFirstSegment;
		}
		set {
			_startsOnFirstSegment = value;
		}
	}

	#endregion

	/// <summary>
	/// Force to apply to the vehicle
	/// </summary>
	/// <returns>
	/// A <see cref="Vector3"/>
	/// </returns>
	protected override Vector3 CalculateForce ()
	{
		Vector3 force = Vector3.zero;
		if (_path != null)
		{
			while (force == Vector3.zero && _currentNode < _path.SegmentCount)
			{
				var node = _path.Path[_currentNode];
				force = Vehicle.GetSeekVector(node, false);
				if (force == Vector3.zero)
				{
					_currentNode++;
				}
			}
		}
		return force;
	}
	
	int FindStartNode()
	{
		int closest = -1;
		if (_startsOnFirstSegment)
		{
			closest = 0;
		}
		else
		{
			float minDistance = float.MaxValue;
			var pos = Vehicle.Position;
			for(int i = 0; i < _path.SegmentCount; i++)
			{
				Vector3 point = _path.Path[i];
				var distance = (point - pos).sqrMagnitude;
				if (distance < minDistance)
				{
					minDistance = distance;
					closest = i;
				}
			}
		}
		return closest;
	}
}
