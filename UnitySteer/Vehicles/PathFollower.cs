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
        
        private float   neighborAvoidanceWeight = 1;
        private float   obstacleAvoidanceWeight = 0;
        private float   pathFollowWeight        = 2;
        
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
        
        public float NeighborAvoidanceWeight
        {
            get
            {
                return neighborAvoidanceWeight;
            }
            set
            {
                neighborAvoidanceWeight = Mathf.Clamp(value, 0, float.MaxValue);
            }
        }
        
        public float ObstacleAvoidanceWeight
        {
            get
            {
                return obstacleAvoidanceWeight;
            }
            set
            {
                obstacleAvoidanceWeight = Mathf.Clamp(value, 0, float.MaxValue);;
            }
        }
        
        public float PathFollowWeight
        {
            get
            {
                return pathFollowWeight;
            }
            set
            {
                pathFollowWeight = Mathf.Clamp(value, 0, float.MaxValue);;
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
        }
                
        // one simulation step
        public void Update(float elapsedTime)
        {
            if (!moving || Pathway == null)
                return;
                
            Vector3 steer = Vector3.zero;

            // Damn I could use a lambda right here
            if (PathFollowWeight != 0)
            {
                Vector3 force = steerToFollowPath(+1, 1, Pathway);
                steer += force  * PathFollowWeight;
            }
            if (ObstacleAvoidanceWeight != 0)
            {
                Vector3 force = steerToAvoidObstacles(MinCollisionTime, Obstacles);
                steer += force * ObstacleAvoidanceWeight;
            }
            if (NeighborAvoidanceWeight != 0)
            {
                Vector3 force = Neighbors.Count > 0 ?
                                    steerToAvoidNeighbors(MinCollisionTime, Neighbors) :
                                    Vector3.zero;
                steer += force * NeighborAvoidanceWeight;
            }

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