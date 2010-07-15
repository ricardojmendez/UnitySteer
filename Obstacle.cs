using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace UnitySteer
{
	public delegate Obstacle ObstacleFactory(GameObject go);
	
    public class Obstacle
    {
        private static Dictionary<int, Obstacle> _obstacleCache;
		
		public static Dictionary<int, Obstacle> ObstacleCache {
			get {
				return _obstacleCache;
			}
		}
		
		static Obstacle()
        {
            _obstacleCache = new Dictionary<int, Obstacle>();
        }		
		
		
		public virtual Vector3 steerToAvoid(SteeringVehicle v, float minTimeToCollision)
		{
			return Vector3.zero;
		}

    }

    public class SphericalObstacle : Obstacle
    {
        public float radius;
        public Vector3 center;


        // constructors
        public SphericalObstacle(float r, Vector3 c)
        {
            radius = r;
            center = c;
        }

        public SphericalObstacle()
        {
            radius = 1;
            center = Vector3.zero;
        }
		
		public override string ToString ()
		{
			return string.Format ("[SphericalObstacle {0} {1}]", center, radius);
		}
        
        /// <summary>
        ///Returns a SphericalObstacle from the current gameObject 
        /// </summary>
        /// <param name="gameObject">
        /// A game object to create the obstacle from<see cref="GameObject"/>
        /// </param>
        /// <returns>
        /// A SphericalObstacle encompassing the game object<see cref="Obstacle"/>
        /// </returns>
        public static Obstacle GetObstacle( GameObject gameObject )
    	{
    		SphericalObstacle obstacle;
    		int id = gameObject.GetInstanceID();
    		Component[] colliders;
    		float radius = 0.0f, currentRadius;

    		if(!ObstacleCache.ContainsKey( id ))
    		{
    			colliders = gameObject.GetComponentsInChildren<Collider>();

    			if( colliders == null )
    			{
    				Debug.LogError( "Obstacle '" + gameObject.name + "' has no colliders" );
    				return null;
    			}

    			foreach( Collider collider in colliders )
    			{
    				if( collider.isTrigger )
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
                    currentRadius = distanceToCollider + maxExtents;
    				if( currentRadius > radius )
    				{
    					radius = currentRadius;
    				}
    			}
    			ObstacleCache[id] = new SphericalObstacle( radius, gameObject.transform.position );
    		}
    		obstacle = ObstacleCache[ id ] as SphericalObstacle;

    		return obstacle;
    	}
		
        

        // XXX 4-23-03: Temporary work around (see comment above)
        //
        // Checks for intersection of the given spherical obstacle with a
        // volume of "likely future vehicle positions": a cylinder along the
        // current path, extending minTimeToCollision seconds along the
        // forward axis from current position.
        //
        // If they intersect, a collision is imminent and this function returns
        // a steering force pointing laterally away from the obstacle's center.
        //
        // Returns a zero vector if the obstacle is outside the cylinder
        //
        // xxx couldn't this be made more compact using localizePosition?
        public override Vector3 steerToAvoid(SteeringVehicle v, float minTimeToCollision)
        {
            // minimum distance to obstacle before avoidance is required
            float minDistanceToCollision = minTimeToCollision * v.Speed;
            float minDistanceToCenter = minDistanceToCollision + radius;

            // contact distance: sum of radii of obstacle and vehicle
             float totalRadius = radius + v.Radius;

            // obstacle center relative to vehicle position
             Vector3 localOffset = center - v.Position;

            // distance along vehicle's forward axis to obstacle's center
             float forwardComponent = Vector3.Dot(localOffset, v.Forward);
             Vector3 forwardOffset = forwardComponent * v.Forward;

            // offset from forward axis to obstacle's center
             Vector3 offForwardOffset = localOffset - forwardOffset;

            // test to see if sphere overlaps with obstacle-free corridor
             bool inCylinder = offForwardOffset.magnitude < totalRadius;
             bool nearby = forwardComponent < minDistanceToCenter;
             bool inFront = forwardComponent > 0;

            // if all three conditions are met, steer away from sphere center
            if (inCylinder && nearby && inFront)
            {
                return offForwardOffset * -1;
            }
            else
            {
                return Vector3.zero;
            }
        }

        public void annotatePosition()
        {
            annotatePosition(Color.grey);
        }
        
        public void annotatePosition(Color color)
        {
			// Primitive sphere position indicator, since Unity lacks a 
			// Debug.DrawSphere
			Debug.DrawRay(center, Vector3.up * radius, color);
			Debug.DrawRay(center, Vector3.forward * radius, color);
			Debug.DrawRay(center, Vector3.right * radius, color);
        }
    }
}


