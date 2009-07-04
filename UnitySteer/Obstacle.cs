using System;
using System.Collections;
using System.Text;
using UnityEngine;

namespace UnitySteer
{
    public class Obstacle
    {
        public enum seenFromState {outside, inside, both};

        public virtual seenFromState seenFrom()
        {
            // Err not sure what best to do here
            return seenFromState.inside;
        }
        public virtual void setSeenFrom(seenFromState s)
        {
        }

        // XXX 4-23-03: Temporary work around (see comment above)
        // CHANGED FROM ABSTRACTVEHICLE. PROBLY SHOULD CHANGE BACK!
        public virtual Vector3 steerToAvoid(System.Object v, float minTimeToCollision)
        {
            return Vector3.zero;
        }

    }

    public class SphericalObstacle : Obstacle
    {
        
        private static Hashtable obstacleCache;

        public float radius;
        public Vector3 center;

        private seenFromState _seenFrom;
        
        static SphericalObstacle()
        {
            obstacleCache = new Hashtable();
        }

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
        
        
        public static SphericalObstacle GetObstacle( GameObject gameObject )
    	{
    		SphericalObstacle obstacle;
    		int id = gameObject.GetInstanceID();
    		Component[] colliders;
    		float radius = 0.0f, currentRadius;

    		if( !obstacleCache.ContainsKey( id ) )
    		{
    			colliders = gameObject.GetComponentsInChildren( typeof( Collider ) );

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

    				currentRadius = Mathf.Abs( ( gameObject.transform.position - ( collider.transform.position + collider.bounds.center ) ).x ) + collider.bounds.extents.x;
    				currentRadius *= gameObject.transform.localScale.x;
    				//currentRadius = gameObject.transform.localScale.x / 2.0f;

    				if( currentRadius > radius )
    				{
    					radius = currentRadius;
    				}
    			}
    			obstacleCache[id] = new SphericalObstacle( radius, gameObject.transform.position );
    		}
    		obstacle = obstacleCache[ id ] as SphericalObstacle;

    		return obstacle;
    	}
        

        public override seenFromState seenFrom() { return _seenFrom; }
        public override void setSeenFrom(seenFromState s) { _seenFrom = s; }

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
        Vector3 steerToAvoid(Vehicle v, float minTimeToCollision)
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
        
    }
}


