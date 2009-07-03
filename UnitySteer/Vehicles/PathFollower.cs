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
        
        public event VehicleArrivalHandler VehicleArrived;

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
                reset();
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
            moving = true;
            MaxForce =  2.0f;   // steering force is clipped to this magnitude
            MaxSpeed =  5.0f;   // velocity is clipped to this magnitude
        }
                
        // one simulation step
        public void Update(float elapsedTime)
        {
            if (!moving || Pathway == null)
                return;

            Vector3 follow = steerToFollowPath(+1, 1, Pathway);
            applySteeringForce(follow, elapsedTime);
            
            
            bool arrived = Vector3.Distance(Position, Pathway.LastPoint) <= Radius;
            
            if (arrived)
            {
                moving = !StopOnArrival;
                if (VehicleArrived != null)
                    VehicleArrived(this, target);
            }
            
        }
    };
}