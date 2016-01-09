using UnityEngine;
using UnitySteer2D.Behaviors;
using UnitySteer.Attributes;

namespace UnitySteer2D.Tools
{
	/// <summary>
	/// Configures a detectable on the game object it is attached to, based
	/// on the boundaries of its child colliders, and then destroys itself.
	/// </summary>
	[AddComponentMenu("UnitySteer2D/Tools/Detectable Object Creator")]
	public class DetectableObjectCreator2D : MonoBehaviour
	{
        void Awake()
		{
			CreateDetectableObject();
			Destroy(this);
		}

		void CreateDetectableObject()
		{
			var radius = 0.0f;

            var colliders = gameObject.GetComponentsInChildren<Collider2D>();

            if (colliders == null)
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
			    float distanceToCollider = Vector2.Distance(gameObject.transform.position, collider.bounds.center);
	            var currentRadius = distanceToCollider + maxExtents;
				if (currentRadius > radius)
				{
					radius = currentRadius;
				}
			}

			var scale  = transform.lossyScale;
			radius /= Mathf.Max(scale.x, Mathf.Max(scale.y, scale.z));

			var detectable = gameObject.AddComponent<DetectableObject2D>();
			detectable.Center = Vector2.zero;
			detectable.Radius = radius;
		}
	}

}