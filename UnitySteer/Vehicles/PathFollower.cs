//#define DEBUG
using UnitySteer;
using UnityEngine;
using System.Collections;


namespace UnitySteer.Vehicles
{
    public delegate void VehicleArrivalHandler(SimpleVehicle vehicle, Vector3 point);
    
    public class PathFollower : SimpleVehicle
    {
        private Pathway pathway;
        private bool    moving        = false;
        private bool    stopOnArrival = true;
        private Vector3 target;
        private float   heightDifference;
        private float   minCollisionTime;
        
        private float   NeighborAvoidanceWeight = 3;
        private float   ObstacleAvoidanceWeight = 3;
        private float   PathFollowWeight        = 1;
        
        private ArrayList neighbors;
        
        public event VehicleArrivalHandler VehicleArrived;
        
        public ArrayList Neighbors
        {
            get
            {
                return neighbors;
            }
        }

        public Pathway Pathway
        {
            get
            {
                return pathway;
            }
            set
            {
                pathway = value;
                moving  = pathway != null;
                if (!MovesVertically && pathway != null)
                {
                    /*
                     * Compensate for the fact that since the vehicle won't 
                     * move vertically, then there will be a certain distance
                     * between the target and how close it can actually get.
                     */
                    heightDifference = Mathf.Abs(Pathway.LastPoint.y - Position.y);
                }
                else
                {
                    heightDifference = 0;
                }
                reset();
            }
        }
        
        public float MinCollisionTime
        {
            get
            {
                return minCollisionTime;
            }
            set
            {
                minCollisionTime = Mathf.Clamp(value, 0, float.MaxValue);
            }
        }
        
        
        public bool Moving
        {
            get
            {
                return moving;
            }
            set
            {
                moving = value;
            }
        }
        
        public bool StopOnArrival
        {
            get
            {
                return stopOnArrival;
            }
            set
            {
                stopOnArrival = value;
            }
        }
        
        
        
        // constructor
		public PathFollower(Transform transform, float mass, Pathway pathway, float radius) : base( transform, mass )
		{ 
		    reset(); 
		    this.Pathway  = pathway;
		    this.Radius   = radius;
		}
		
		public PathFollower(Rigidbody rigidbody, Pathway pathway, float radius) : base( rigidbody )
		{ 
		    reset(); 
		    this.Pathway  = pathway;
		    this.Radius   = radius;
		}

        new void reset ()
        {
            base.reset (); // reset the vehicle 
            neighbors = new ArrayList();
            moving = true;
            MaxForce =  2.0f;   // steering force is clipped to this magnitude
            MaxSpeed =  5.0f;   // velocity is clipped to this magnitude
        }
                
        // one simulation step
        public void Update(float elapsedTime)
        {
            if (!moving || Pathway == null)
                return;
                
            Vector3 steer = Vector3.zero;
            float   total = 0;

            // Damn I could use a lambda right here
            if (PathFollowWeight != 0)
            {
                Vector3 force = steerToFollowPath(+1, 1, Pathway);
                if (force != Vector3.zero)
                {
                    steer += force;
                }
                total += PathFollowWeight;
            }
            if (ObstacleAvoidanceWeight != 0)
            {
                Vector3 force = steerToAvoidObstacles(MinCollisionTime, Obstacles);
                if (force != Vector3.zero)
                {
                    steer += force;
                    total += ObstacleAvoidanceWeight;
                }
            }
            if (NeighborAvoidanceWeight != 0)
            {
                Vector3 force = Neighbors.Count > 0 ?
                                    steerToAvoidNeighbors(MinCollisionTime, Neighbors) :
                                    Vector3.zero;
                if (force != Vector3.zero)
                {
                    steer += force;
                    total += NeighborAvoidanceWeight;
                }
            }

            if (total == 0)
                return;
            steer /= total;
            applySteeringForce(steer, elapsedTime);
            
            bool arrived = Vector3.Distance(Position, Pathway.LastPoint) <= Radius + heightDifference;
            
            if (arrived)
            {
                moving = !StopOnArrival;
                if (VehicleArrived != null)
                    VehicleArrived(this, target);
            }
            
        }
    };
}