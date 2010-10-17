using UnityEngine;
using System.Collections;

/// <summary>
/// Defines the data for a spherical obstacle that should be used to override
/// the object's bounds calculation.
/// </summary>
/// <remarks>
/// While the automatic bounds calculation performed by SphericalObstacle.GetObstacle
/// is very useful, it aims to encompass the whole object in a sphere. As
/// such, objects that are very tall or very wide will end up returning 
/// unnecessarily large bounding spheres. 
/// Adding a SphericalObstacleData behavior to any object allows the developer
/// to override these bounds calculations with an obstacle that's precisely 
/// positioned for the area we wish to block off. For example, if our 
/// agents are gravity-bound we care only about considering the trunks of 
/// trees as obstacles, not the whole figure.
/// </remarks>
public class SphericalObstacleData : MonoBehaviour {
	
	[SerializeField]
	Vector3 _center = Vector3.zero;
	
	[SerializeField]	
	float _radius = 1;

	
	/// <summary>
	/// The obstacle's center relative to the transform's position
	/// </summary>
	public Vector3 Center {
		get {
			return this._center;
		}
	}

	/// <summary>
	/// The obstacle's radius
	/// </summary>
	/// <value>
	/// The radius.
	/// </value>
	public float Radius {
		get {
			return this._radius;
		}
	}
	
	
	void OnDrawGizmosSelected() {
		Gizmos.DrawWireSphere(transform.position + Center, Radius);
	}
}
