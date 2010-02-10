//#define DEBUG
using UnitySteer;
using UnityEngine;
using System.Collections;

/// <summary>
/// Simple vehicle for moving to a point
/// </summary>
/// <remarks>
//  Very simple vehicle that moves to whatever point it was told to reach and,
//  once there, says at that point.  Added neighbor and obstacle avoidance for 
/// testing purposes.
/// 
/// Notice that UnitySteer is not meant to be the final product, but instead a
/// library of steering behaviors that people can then assemble into the 
/// vehicles they need. As such, this example vehicle is trivial and focuses
/// solely on demonstrating how to integrate the steering functions into a 
/// cohesive vehicle.
/// </remarks>
namespace UnitySteer.Vehicles
{
    public class MovesToPoint : SimpleVehicle
    {
        private Vector3 target;
        private bool moving = false;

		public float MinCollisionTime = 3;
		public float NeighborAvoidanceWeight = 2;
		public float ObstacleAvoidanceWeight = 0.5f;

        public Vector3 Target
        {
            get
            {
                return target;
            }
            set
            {
                if (target != value)
                {
                    target = value;
                    moving = true;
                }
            }
        }

        // constructor
		public MovesToPoint( Transform transform, float mass, Vector3 target, float radius ) : base( transform, mass )
		{
		    reset();
		    this.target = target;
		    this.Radius = radius;
		}

		public MovesToPoint( Rigidbody rigidbody, Vector3 target, float radius ) : base( rigidbody )
		{
		    reset();
		    this.target = target;
		    this.Radius = radius;
		}

        // reset state
        new void reset ()
        {
            base.reset (); // reset the vehicle
            moving = true;
            MaxForce =  2.0f;   // steering force is clipped to this magnitude
            MaxSpeed = 13.0f;   // velocity is clipped to this magnitude
        }

        // one simulation step
        public void Update (float elapsedTime)
        {
            if (!moving)
                return;

			/*
			 * First off, we calculate how far we are from the target, If this
			 * distance is smaller than the configured vehicle radius, we tell
			 * the vehicle to stop.
			 */
            float d = Vector3.Distance(Position, target);
            float r = Radius;
            if (d < r)
			{
                this.moving = false;
				return;
			}
			
			/*
			 * But suppose we still have some distance to go. The first step
			 * then would be calculating the steering force necessary to orient
			 * ourselves to and walk to that point.  The steerForSeek function
			 * takes into account values luke the MaxForce to apply and the 
			 * vehicle's MaxSpeed, and returns a steering vector.
			 * 
			 * It doesn't apply the steering itself, simply returns the value so
			 * we can continue operating on it.
			 */
            Vector3 seeking = steerForSeek (target);

			/*
			 * We are not done yet.  We then need to evaluate if we have been
			 * configured to avoid obstacles and neighbors. If so, we calculate
			 * the respective vectors, and blend them with the one for the point
			 * we're seeking.
			 * 
			 * Notice that the neighbor and obstacle lists are expected to be
			 * pre-filled, and are not calculated by these functions.
			 * 
			 */
            if (NeighborAvoidanceWeight != 0)
            {
                Vector3 force = Neighbors.Count > 0 ?
                                    steerToAvoidNeighbors(MinCollisionTime, Neighbors) :
                                    Vector3.zero;
                seeking += force * NeighborAvoidanceWeight;
            }

			if (ObstacleAvoidanceWeight != 0)
			{
				Vector3 avoid = steerToAvoidObstacles(2, Obstacles);
				seeking += avoid * ObstacleAvoidanceWeight;
			}

			/*
			 * Finally, we apply the resulting steering vector, which should
			 * avoid any nearby objects while taking us towards our goal.
			 */
            applySteeringForce (seeking, elapsedTime);

            #if DEBUG
            Debug.DrawLine(Transform.position, target, Color.green);
            Debug.DrawLine(Transform.position, seeking, Color.blue);
            #endif
        }

    };
}