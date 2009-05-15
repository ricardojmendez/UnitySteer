using OpenSteer;
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


namespace OpenSteer.Vehicles
{

    // ----------------------------------------------------------------------------
    // This PlugIn uses two vehicle types: MpWanderer and MpPursuer.  They have
    // a common base class, MpBase, which is a specialization of SimpleVehicle.


    public class MpBase : SimpleVehicle
    {
        // constructor
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
		public MpWanderer( Transform transform, float mass ) : base( transform, mass ){ reset(); }
		public MpWanderer( Rigidbody rigidbody ) : base( rigidbody ){ reset(); }
        

        // reset state
        new void reset ()
        {
            base.reset ();
        }

        // one simulation step
        public void update (float currentTime, float elapsedTime)
        {
            Vector3 wander2d = steerForWander (elapsedTime);
            wander2d.y = 0;
            Vector3 steer = Forward + (wander2d * 3);
            applySteeringForce (steer, elapsedTime);

            // for annotation
            // recordTrailVertex (currentTime, Position);
        }

    };


    public class MpPursuer : MpBase
    {
        MpWanderer wanderer;

		public MpPursuer( Transform transform, float mass, MpWanderer w ) : base( transform, mass )
		{
            wanderer = w; 
            reset ();
		}
		
		public MpPursuer( Rigidbody rigidbody, MpWanderer w ) : base( rigidbody )
		{
            wanderer = w; 
            reset ();
		}

        // reset state
        new void reset ()
        {
            base.reset ();
            randomizeStartingPositionAndHeading ();
        }

        // one simulation step
        public void update (float currentTime, float elapsedTime)
        {
            // when pursuer touches quarry ("wanderer"), reset its position
            float d = Vector3.Distance(Position, wanderer.Position);
            float r = Radius + wanderer.Radius;
            if (d < r) reset ();

            float maxTime = 20f; // xxx hard-to-justify value
            applySteeringForce (steerForPursuit (wanderer, maxTime), elapsedTime);

            // for annotation TODO-REMOVE
            // recordTrailVertex (currentTime, Position);
        }

        // reset position
        void randomizeStartingPositionAndHeading ()
        {
            // randomize position on a ring between inner and outer radii
            // centered around the home base
            float inner = 20;
            float outer = 30;
            float radius = Random.Range(inner, outer);
            
            Vector3 r  = Random.insideUnitSphere;
            r.y = 0;
            
            Vector3 randomOnRing = r * radius;
            Position = wanderer.Position + randomOnRing;

            // randomize 2D heading
//            randomizeHeadingOnXZPlane ();
				// TODO: Check consequences. Figure something out, dude!
        }
    };

}
