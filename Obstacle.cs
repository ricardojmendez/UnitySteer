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
				var obstacleData = gameObject.GetComponent<SphericalObstacleData>();
				// If the object provides his own spherical obstacle information,
				// use it instead of calculating a sphere that encompasses the 
				// whole collider.
				if (obstacleData != null) 
				{
					ObstacleCache[id] = new SphericalObstacle(obstacleData.Radius, gameObject.transform.position + obstacleData.Center);
				}
				else 
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
    		}
    		obstacle = ObstacleCache[ id ] as SphericalObstacle;

    		return obstacle;
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


