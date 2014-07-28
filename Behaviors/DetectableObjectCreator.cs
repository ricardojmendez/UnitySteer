using UnityEngine;
using UnitySteer.Behaviors;

namespace UnitySteer.Helpers
{

/// <summary>
/// Configures a detectable on the game object it is attached to, based
/// on its boundaries, and then destroys itself.
/// static objects.
/// </summary>
[AddComponentMenu("UnitySteer/Detectables/DetectableObjectCreator")]
public class DetectableObjectCreator : MonoBehaviour
{
	void Awake()
	{
		CreateDetectableObject();
		Destroy(this);
	}


	void CreateDetectableObject()
	{
		float radius = 0.0f;

		var colliders = gameObject.GetComponentsInChildren<Collider>();
		if( colliders == null )
		{
			Debug.LogError(string.Format("Obstacle {0} has no colliders", gameObject.name));
			return;
		}

		foreach (var collider in colliders)
		{
			if(collider.isTrigger)
			{
				continue;
			}
			// Get the maximum extent to create a sphere that encompasses the whole obstacle
			float maxExtents = Mathf.Max(Mathf.Max(collider.bounds.extents.x, collider.bounds.extents.y),
			                             collider.bounds.extents.z);

		    /*
		     * Calculate the displacement from the object center to the 
		     * collider, and add in the maximum extents of the bounds.
		     * Notice that we don't need to multiply by the object's 
		     * local scale, since that is already considered in the 
		     * bounding rectangle.
		     */
		    float distanceToCollider = Vector3.Distance(gameObject.transform.position, collider.bounds.center);
            var currentRadius = distanceToCollider + maxExtents;
			if( currentRadius > radius )
			{
				radius = currentRadius;
			}
		}

		var scale  = transform.lossyScale;
		radius /= Mathf.Max(scale.x, Mathf.Max(scale.y, scale.z));

		var detectable = gameObject.AddComponent<DetectableObject>();
		detectable.Center = Vector3.zero;
		detectable.Radius = radius;
	}
}

}