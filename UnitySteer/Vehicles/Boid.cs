
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
// OpenSteer Boids
// 
// 09-26-02 cwr: created 
//
//
// ----------------------------------------------------------------------------


using UnityEngine;
using System.Collections;
using UnitySteer;

namespace UnitySteer.Vehicles {

    public class Boid : SimpleVehicle
    {

        // allocate one and share amoung instances just to save memory usage
        // (change to per-instance allocation to be more MP-safe)
        static float worldRadius;
        static int boundaryCondition;
        
        public float separationRadius =  5.0f;
        public float separationAngle  = -0.707f;
        public float separationWeight =  12.0f;

        public float alignmentRadius = 7.5f;
        public float alignmentAngle  = 0.7f;
        public float alignmentWeight = 8.0f;

        public float cohesionRadius = 9.0f;
        public float cohesionAngle  = -0.15f;
        public float cohesionWeight = 8.0f;
        
        public Boid (Vector3 position, float mass, bool movesVertically) : base( position, mass )
        {
            this.MovesVertically = movesVertically;

            // reset all boid state
            reset ();
        }
        
        public Boid (Transform transform, float mass, bool movesVertically) : base( transform, mass )
        {
            this.MovesVertically = movesVertically;

            // reset all boid state
            reset ();
        }

        public Boid ( Rigidbody rigidbody, bool movesVertically) : base( rigidbody )
        {
            this.MovesVertically = movesVertically;

            // reset all boid state
            reset ();
        }
        
        // constructor
        public Boid (Transform transform, float mass): this(transform, mass, true){}
        public Boid (Rigidbody rigidbody): this(rigidbody, true){}


        // cycle through various boundary conditions
        static void nextBoundaryCondition ()
        {
            const int max = 2;
            boundaryCondition = (boundaryCondition + 1) % max;
        }
        


        // reset state
        new void reset ()
        {
            // reset the vehicle
            base.reset();
            // initial slow speed
            Speed = MaxSpeed * 0.3f;
        }
        
        
        public void Randomize(float distance)
        {
            // randomize initial orientation
	        Forward = Random.insideUnitCircle;
            // randomize initial position
            Position = Random.insideUnitSphere * distance;
        }
        
        
        protected virtual Vector3 CalculateForces()
        {
            // steer to flock and perhaps to stay within the spherical boundary
            Vector3 avoid = steerToAvoidObstacles(0.2f, Obstacles); // TODO-RJM: Change to a property
            #if DEBUG
            if (Obstacles.Count > 0 && avoid != Vector3.zero)
            {
                Debug.Log("Avoiding "+avoid);
            }
            #endif
            Vector3 result = steerToFlock () + handleBoundary() + avoid;
            return result;
        }
        
        // per frame simulation update
        public void Update (float currentTime, float elapsedTime)
        {
            Vector3 forces = CalculateForces();
            
            /*
            if (forces != Vector3.zero)
                Debug.Log("Forces "+forces+ " "+ elapsedTime);
            */
            applySteeringForce (forces, elapsedTime);
        }


        // basic flocking
        Vector3 steerToFlock ()
        {
            // determine each of the three component behaviors of flocking
            Vector3 separation = steerForSeparation (separationRadius,
                                                     separationAngle,
                                                     Neighbors);
            Vector3 alignment  = steerForAlignment  (alignmentRadius,
                                                     alignmentAngle,
                                                     Neighbors);
            Vector3 cohesion   = steerForCohesion   (cohesionRadius,
                                                     cohesionAngle,
                                                     Neighbors);

            // apply weights to components (save in variables for annotation)
            Vector3 separationW = separation * separationWeight;
            Vector3 alignmentW = alignment * alignmentWeight;
            Vector3 cohesionW = cohesion * cohesionWeight;

            // annotation
            // const float s = 0.1;
            // annotationLine (position, position + (separationW * s), gRed);
            // annotationLine (position, position + (alignmentW  * s), gOrange);
            // annotationLine (position, position + (cohesionW   * s), gYellow);

            return separationW + alignmentW + cohesionW;
        }


        // Take action to stay within sphereical boundary.  Returns steering
        // value (which is normally zero) and may take other side-effecting
        // actions such as kinematically changing the Boid's position.
        Vector3 handleBoundary ()
        {
            // while inside the sphere do noting
            if (Position.magnitude < worldRadius) return Vector3.zero;

            // once outside, select strategy
            switch (boundaryCondition)
            {
                case 0:
                {
                    // steer back when outside
                    Vector3 seek = xxxsteerForSeek (Vector3.zero);
                    Vector3 lateral = OpenSteerUtility.perpendicularComponent(seek, Forward);
                    return lateral;
                }
                case 1:
                {
                    // wrap around (teleport)
                    /* TODO-CHECK
                    setPosition (Position.sphericalWrapAround (Vector3.zero,
                                                                 worldRadius));
                    */
                    return Vector3.zero;
                }
            }
            return Vector3.zero; // should not reach here
        }


        // make boids "bank" as they fly
        void regenerateLocalSpace (Vector3 newVelocity,
                                   float elapsedTime)
        {
	// TODO: Put back in?
//            regenerateLocalSpaceForBanking (newVelocity, elapsedTime);
        }


    };
}
// ----------------------------------------------------------------------------
