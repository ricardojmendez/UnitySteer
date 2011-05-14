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
/// 
/// The vast majority of the functionality is now implemented as part of 
/// DetectableObject. Retaining this class for backwards compatibility.
/// </remarks>
[AddComponentMenu("UnitySteer/Detectables/Spherical Obstacle Data")]
public class SphericalObstacleData : DetectableObject {
	
	
	void OnDrawGizmosSelected() {
		Gizmos.DrawWireSphere(transform.position + Center, Radius);
	}
}
