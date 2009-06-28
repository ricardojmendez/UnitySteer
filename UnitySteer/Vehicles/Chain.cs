using UnitySteer;
using UnityEngine;
using System.Collections;

// ----------------------------------------------------------------------------
//
// Ported to Unity and .Net by Ricardo J. Méndez http://www.arges-systems.com/
//
// Based on OpenSteer -- Steering Behaviors for Autonomous Characters
//
// ----------------------------------------------------------------------------


namespace UnitySteer.Vehicles
{

    // ----------------------------------------------------------------------------
    // This PlugIn uses two vehicle types: MpWanderer and MpPursuer.  They have
    // a common base class, MpBase, which is a specialization of SimpleVehicle.


    public class Chain : SimpleVehicle
    {
        private SimpleVehicle previous, next;
        private float previousStrength = 1f;
        private float nextStrength     = 1f;
        
        public SimpleVehicle Previous
        {
            get
            {
                return previous;
            }
            
            set
            {
                previous = value;
            }
        }
        
        public SimpleVehicle Next
        {
            get
            {
                return next;
            }
            set
            {
                next = value;
            }
        }
        
        
        public float PreviousStrength
        {
            get
            {
                return previousStrength;
            }
            set
            {
                previousStrength = value;
            }
        }
        
        public float NextStrength
        {
            get
            {
                return nextStrength;
            }
            set
            {
                nextStrength = value;
            }
        }
        
        
        public Chain (Vector3 position, float mass, SimpleVehicle previous, SimpleVehicle next) : base(position, mass)
        {
            this.previous = previous;
            this.next = next;
            reset ();
        }


        // one simulation step
        public void Update (float elapsedTime)
        {
            if (next == null)
            {
                return;
            }
            // when pursuer touches quarry ("next"), reset its position
            float d = Vector3.Distance(Position, next.Position);
            float r = Radius + next.Radius;

            #if RESET_ON_FIND
            if (d < r) reset ();
            #endif

            if (d >= r)
            {
                float maxTime = 20f; // xxx hard-to-justify value
                
                Vector3 pursuit = steerForPursuit (next, maxTime);
                Vector3 pull = Vector3.zero;
                
                if (previous != null)
                {
                    pull =  steerForPursuit(previous, maxTime);
                    pull *= PreviousStrength;
                }
                pursuit *= NextStrength;
                
                applySteeringForce ( pursuit + pull, elapsedTime);
            }
        }
        
        private new void reset ()
        {
            base.reset ();      // reset the vehicle 
            Speed = 5.0f;       // speed along Forward direction.
            MaxForce = 15.0f;   // steering force is clipped to this magnitude
            MaxSpeed = 13.0f;   // velocity is clipped to this magnitude
        }
        
        

        // reset position
        public void randomizeStartingPositionAndHeading ()
        {
            // randomize position on a ring between inner and outer radii
            // centered around the home base
            float inner = 20;
            float outer = 30;
            float radius = Random.Range(inner, outer);

            Vector3 r  = Random.insideUnitSphere;
            r.y = 0;

            Vector3 randomOnRing = r * radius;
            Position = next.Position + randomOnRing;
        }
    };

}
