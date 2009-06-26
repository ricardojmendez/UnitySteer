using UnitySteer;
using UnityEngine;
using System.Collections;

// ----------------------------------------------------------------------------
//
// Ported to Unity and .Net by Ricardo J. Méndez http://www.arges-systems.com/
//
// OpenSteer -- Steering Behaviors for Autonomous Characters
//
// Copyright (c) 2002-2003, Sony Computer Entertainment America
// Original author: Craig Reynolds <craig_reynolds@playstation.sony.com>
//
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.  IN NO EVENT SHALL
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.
//
//
// ----------------------------------------------------------------------------
//
//
// Multiple pursuit (for testing pursuit)
//
// 08-22-02 cwr: created 
//
//
// ----------------------------------------------------------------------------


namespace UnitySteer.Vehicles
{

    // ----------------------------------------------------------------------------
    // This PlugIn uses two vehicle types: MpWanderer and MpPursuer.  They have
    // a common base class, MpBase, which is a specialization of SimpleVehicle.


    public class MpBase : SimpleVehicle
    {
        // constructor
        public MpBase( Vector3 position, float mass ) : base( position, mass )
		{
            reset ();
		}
		public MpBase( Transform transform, float mass ) : base( transform, mass ){ reset(); }
		public MpBase( Rigidbody rigidbody ) : base( rigidbody ){ reset(); }

        // reset state
        new void reset ()
        {
            base.reset (); // reset the vehicle 
            Speed = 5.0f;            // speed along Forward direction.
            MaxForce = 55.0f;       // steering force is clipped to this magnitude
            MaxSpeed = 13.0f;       // velocity is clipped to this magnitude
            // TODO-REMOVE
            // clearTrailHistory ();    // prevent long streaks due to teleportation 
            // gaudyPursuitAnnotation = true; // select use of 9-color annotation
        }
    };


    public class MpWanderer : MpBase
    {
        // constructor
		public MpWanderer( Transform transform, float mass, bool movesVertically ) : base( transform, mass )
		{
		    this.MovesVertically = movesVertically;
		    reset(); 
		}
		
		public MpWanderer( Rigidbody rigidbody, bool movesVertically ) : base( rigidbody )
		{
		    this.MovesVertically = movesVertically;
		    reset(); 
		}
        

        // reset state
        new void reset ()
        {
            base.reset ();
        }

        // one simulation step
        public void Update (float currentTime, float elapsedTime)
        {
            Vector3 wander = steerForWander (elapsedTime);
            // Vector3 steer = Forward + (wander * 3);
            Vector3 steer = wander;
            applySteeringForce (steer, elapsedTime);

            // for annotation
            // recordTrailVertex (currentTime, Position);
        }

    };


    public class MpPursuer : MpBase
    {
        SimpleVehicle targetVehicle;
        
        
        public SimpleVehicle TargetVehicle
        {
            get
            {
                return targetVehicle;
            }
            
            set
            {
                targetVehicle = value;
            }
        }
        

		public MpPursuer( Vector3 position, float mass, SimpleVehicle w ) : base( position, mass )
		{
            targetVehicle = w; 
            reset ();
		}


		public MpPursuer( Transform transform, float mass, SimpleVehicle w ) : base( transform, mass )
		{
            targetVehicle = w; 
            reset ();
		}

		public MpPursuer( Rigidbody rigidbody, SimpleVehicle w ) : base( rigidbody )
		{
            targetVehicle = w; 
            reset ();
		}

        // one simulation step
        public void update (float currentTime, float elapsedTime)
        {
            if (targetVehicle == null)
            {
                return;
            }
            // when pursuer touches quarry ("targetVehicle"), reset its position
            float d = Vector3.Distance(Position, targetVehicle.Position);
            float r = Radius + targetVehicle.Radius;

            #if RESET_ON_FIND
            if (d < r) reset ();
            #endif

            if (d >= r)
            {
                float maxTime = 20f; // xxx hard-to-justify value
                applySteeringForce (steerForPursuit (targetVehicle, maxTime), elapsedTime);
            }
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
            Position = targetVehicle.Position + randomOnRing;
        }
    };

}
