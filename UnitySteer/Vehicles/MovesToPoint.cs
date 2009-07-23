//#define DEBUG
using UnitySteer;
using UnityEngine;
using System.Collections;

// ----------------------------------------------------------------------------
//
// Very simple vehicle that moves to whatever point it was told to reach and,
// once there, says at that point.  Added neighbor avoidance for testing 
// purposes.
//
// ----------------------------------------------------------------------------



namespace UnitySteer.Vehicles
{
    public class MovesToPoint : SimpleVehicle
    {
        private Vector3 target;
        private bool moving = false;
        
        public float MinCollisionTime = 3;
        public float NeighborAvoidanceWeight = 2;
        
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

            float d = Vector3.Distance(Position, target);
            float r = Radius;
            if (d < r) 
                this.moving = false;
            
            Vector3 seeking = steerForSeek (target);
            
            if (NeighborAvoidanceWeight != 0)
            {
                Vector3 force = Neighbors.Count > 0 ?
                                    steerToAvoidNeighbors(MinCollisionTime, Neighbors) :
                                    Vector3.zero;
                seeking += force * NeighborAvoidanceWeight;
            }

            applySteeringForce (seeking, elapsedTime);

            #if DEBUG
            Debug.DrawLine(Transform.position, target, Color.green);
            Debug.DrawLine(Transform.position, seeking, Color.blue);
            #endif
        }
        
    };
}