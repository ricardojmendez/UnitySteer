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
        private float previousStrength  = 1;
        private float nextStrength      = 1;
        private float maxDistance       = 1;
        private float maxDistanceSqr    = 1;
        
        public float MaxDistance
        {
            get
            {
                return maxDistance;
            }
            set
            {
                maxDistance = value;
                maxDistanceSqr = value*value;
            }
        }
        
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

            // Temporary values since we may need to alter them
            float nextStr = NextStrength;
            float prevStr = PreviousStrength;
            
            if (d >= r)
            {
                Vector3 pursuit = steerForPursuit (next);
                Vector3 pull = Vector3.zero;
                
                if (previous != null)
                {
                    Vector3 diff = Position - previous.Position;
                    float   dist = diff.sqrMagnitude;
                    if (dist > maxDistanceSqr)
                    {
                        // If we're further away from the previous link than
                        // we should, change our priorities so that we snap
                        // right back to it.
                        prevStr = 1.95f;
                        nextStr = 0.05f;
                        pull = steerForSeek(previous.Position);
                    }
                    else
                        pull =  steerForPursuit (previous);
                    pull *= prevStr;
                }
                pursuit *= nextStr;
                
                applySteeringForce (pursuit + pull, elapsedTime);
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
