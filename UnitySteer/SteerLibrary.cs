// ----------------------------------------------------------------------------
//
// Ported to Unity by Ricardo J. Méndez http://www.arges-systems.com/
//
// OpenSteer - pure .net port
// Port by Simon Oliver - http://www.handcircus.com
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
//#define DEBUG
#define ANNOTATE_AVOIDOBSTACLES

using System;
using System.Collections;
using System.Text;
using UnityEngine;


namespace UnitySteer
{
    public class SteerLibrary : Vehicle
    {
        // Randomiser
        System.Random randomGenerator;

        // Wander behavior
        private float WanderSide;
        private float WanderUp;

        /// XXX globals only for the sake of graphical annotation
        Vector3 hisPositionAtNearestApproach;
        Vector3 ourPositionAtNearestApproach;

        bool gaudyPursuitAnnotation;
        
        private Transform tether;
        private float     maxDistance;
        private float     maxDistanceSquared;

        public float MaxDistance
        {
            get
            {
                return maxDistance;
            }
            set
            {
                maxDistance = value;
                maxDistanceSquared = maxDistance * maxDistance;
            }
        }
        
        public Transform Tether
        {
            get
            {
                return tether;
            }
            set
            {
                tether = value;
            }
        }
        
        
        public struct PathIntersection
        {
            public bool intersect;
            public float distance;

            // The two below are not used??

            //public Vector3 surfacePoint;
            //public Vector3 surfaceNormal;
            public SphericalObstacle obstacle;
            
            public PathIntersection(SphericalObstacle obstacle)
            {
                this.obstacle = obstacle;
                intersect = false;
                distance = float.MaxValue;
            }
        };

        public SteerLibrary( Vector3 position, float mass ) : base( position, mass ){}
		public SteerLibrary( Transform transform, float mass ) : base( transform, mass ){}
		public SteerLibrary( Rigidbody rigidbody ) : base( rigidbody ){}
			// TODO: Consider doing a call to resetSteering() within these?
        

        // reset state
        public void resetSteering ()
        {
            // initial state of wander behavior
            WanderSide = 0;
            WanderUp = 0;

            // default to non-gaudyPursuitAnnotation
            gaudyPursuitAnnotation = true;

            randomGenerator=new System.Random();
        }

        bool isAhead ( Vector3 target)  {return isAhead (target, 0.707f);}
        bool isAside ( Vector3 target)  {return isAside (target, 0.707f);}
        bool isBehind ( Vector3 target)  {return isBehind (target, -0.707f);}

        bool isAhead ( Vector3 target, float cosThreshold) 
        {
            Vector3 targetDirection = (target - Position);
            targetDirection.Normalize ();
            return (Vector3.Dot(Forward, targetDirection) > cosThreshold);
        }
        
        bool isAside ( Vector3 target, float cosThreshold) 
        {
            Vector3 targetDirection = (target - Position);
            targetDirection.Normalize ();
            float dp = Vector3.Dot(Forward, targetDirection);
            return (dp < cosThreshold) && (dp > -cosThreshold);
        }
        
        bool isBehind ( Vector3 target, float cosThreshold) 
        {
            Vector3 targetDirection = (target - Position);
            targetDirection.Normalize ();
            return Vector3.Dot(Forward, targetDirection) < cosThreshold;
        }

            // called when steerToAvoidObstacles decides steering is required
        // (default action is to do nothing, layered classes can overload it)
        public virtual void annotateAvoidObstacle ( float minDistanceToCollision)
        {
        }

        // called when steerToFollowPath decides steering is required
        // (default action is to do nothing, layered classes can overload it)
        public virtual void annotatePathFollowing (Vector3 future, Vector3 onPath, Vector3 target, float outside)
        {
        }

        // called when steerToAvoidCloseNeighbors decides steering is required
        // (default action is to do nothing, layered classes can overload it)
        public virtual void annotateAvoidCloseNeighbor(Vehicle otherVehicle, float seperationDistance)
        {
        }

        // called when steerToAvoidNeighbors decides steering is required
        // (default action is to do nothing, layered classes can overload it)
        public virtual void annotateAvoidNeighbor (  Vehicle vehicle, float steer,Vector3 position, Vector3 threatPosition)
        {
        }

        public Vector3 steerForWander (float dt)
        {
            // random walk WanderSide and WanderUp between -1 and +1
            float speed = 12 * dt; // maybe this (12) should be an argument?
            
            if (Tether != null)
            {
                Vector3 future  = predictFuturePosition(dt);
                Vector3 diff    = future - Tether.position;
                float   sqrDist = diff.sqrMagnitude;

                if (sqrDist > maxDistanceSquared)
                {
                    //Debug.Log("We're getting far... "+sqrDist);
                    // When we're getting too far away, head back to base
                    return steerForSeek(Tether.position);
                }
            }

            WanderSide = scalarRandomWalk (WanderSide, speed, -1, +1);
            WanderUp   = scalarRandomWalk (WanderUp,   speed, -1, +1);
            
            // return a pure lateral steering vector: (+/-Side) + (+/-Up)
            Vector3  result = (Side * WanderSide) + (Up * WanderUp);
            // result = result * 10;
            //Debug.Log("Wander "+dt+" "+speed+" "+result);
            return result;
        }


        public Vector3 steerForSeek(Vector3 target)
        {
            Vector3 desiredVelocity = target - Position;
            return desiredVelocity - Velocity;
        }


        // ----------------------------------------------------------------------------
        // Flee behavior



        public Vector3 steerForFlee(Vector3 target)
        {
            Vector3 desiredVelocity = Position- target;
            return desiredVelocity - Velocity;
        }


        // ----------------------------------------------------------------------------
        // xxx proposed, experimental new seek/flee [cwr 9-16-02]



        public Vector3 steerForFleeTruncated(Vector3 target)
        {
        //   Vector3 offset = position - target;
            Vector3 offset = Position - target;
            Vector3 desiredVelocity = truncateLength(offset, MaxSpeed);// offset.truncateLength(MaxSpeed); //xxxnew
            return desiredVelocity - Velocity;
        }


       
        public Vector3 steerForSeekTruncated ( Vector3 target)
        {
            Vector3 offset = target - Position;
            Vector3 desiredVelocity = truncateLength(offset, MaxSpeed);// offset.truncateLength(MaxSpeed); //xxxnew
            return desiredVelocity - Velocity;
        }


        // ----------------------------------------------------------------------------
        // Path Following behaviors



        public Vector3 steerToStayOnPath(float predictionTime, Pathway path)
        {
            // predict our future position
            Vector3 futurePosition = predictFuturePosition (predictionTime);

            // find the point on the path nearest the predicted future position
            //Vector3 tangent;
            //float outside;

            mapReturnStruct tStruct = new mapReturnStruct();

            Vector3 onPath = path.mapPointToPath(futurePosition, tStruct);

            if (tStruct.outside < 0)
            {
                // our predicted future position was in the path,
                // return zero steering.
                return Vector3.zero;
            }
            else
            {
                // our predicted future position was outside the path, need to
                // steer towards it.  Use onPath projection of futurePosition
                // as seek target
                annotatePathFollowing (futurePosition, onPath, onPath,tStruct.outside);
                return steerForSeek (onPath);
            }
        }


        public Vector3 steerToFollowPath(int direction, float predictionTime, Pathway path)
        {
            // our goal will be offset from our path distance by this amount
            float pathDistanceOffset = direction * predictionTime * Speed;

            // predict our future position
            Vector3 futurePosition = predictFuturePosition (predictionTime);

            // measure distance along path of our current and predicted positions
            float nowPathDistance =
                path.mapPointToPathDistance (Position);
            float futurePathDistance =
                path.mapPointToPathDistance (futurePosition);

            // are we facing in the correction direction?
            bool rightway = ((pathDistanceOffset > 0) ?
                                   (nowPathDistance < futurePathDistance) :
                                   (nowPathDistance > futurePathDistance));

            // find the point on the path nearest the predicted future position
            // XXX need to improve calling sequence, maybe change to return a
            // XXX special path-defined object which includes two Vector3s and a 
            // XXX bool (onPath,tangent (ignored), withinPath)
            // Vector3 tangent;
            //float outside;
            mapReturnStruct tStruct = new mapReturnStruct();
            Vector3 onPath = path.mapPointToPath(futurePosition, tStruct);

            // no steering is required if (a) our future position is inside
            // the path tube and (b) we are facing in the correct direction
            if ((tStruct.outside < 0) && rightway)
            {
                // all is well, return zero steering
                return Vector3.zero;
            }
            else
            {
                // otherwise we need to steer towards a target point obtained
                // by adding pathDistanceOffset to our current path position

                float targetPathDistance = nowPathDistance + pathDistanceOffset;
                Vector3 target = path.mapPathDistanceToPoint (targetPathDistance);

                annotatePathFollowing(futurePosition, onPath, target, tStruct.outside);

                // return steering to seek target on path
                return steerForSeek (target);
            }
        }


        // ----------------------------------------------------------------------------
        // Obstacle Avoidance behavior
        //
        // Returns a steering force to avoid a given obstacle.  The purely lateral
        // steering force will turn our vehicle towards a silhouette edge of the
        // obstacle.  Avoidance is required when (1) the obstacle intersects the
        // vehicle's current path, (2) it is in front of the vehicle, and (3) is
        // within minTimeToCollision seconds of travel at the vehicle's current
        // velocity.  Returns a zero vector value (Vector3::zero) when no avoidance is
        // required.
        //
        // XXX The current (4-23-03) scheme is to dump all the work on the various
        // XXX Obstacle classes, making them provide a "steer vehicle to avoid me"
        // XXX method.  This may well change.
        //
        // XXX 9-12-03: this routine is probably obsolete: its name is too close to
        // XXX the new steerToAvoidObstacles and the arguments are reversed
        // XXX (perhaps there should be another version of steerToAvoidObstacles
        // XXX whose second arg is "const Obstacle& obstacle" just in case we want
        // XXX to avoid a non-grouped obstacle)

       
        public Vector3 steerToAvoidObstacle ( float minTimeToCollision, Obstacle obstacle)
        {
            Vector3 avoidance = obstacle.steerToAvoid (this, minTimeToCollision);

            // XXX more annotation modularity problems (assumes spherical obstacle)
            if (avoidance != Vector3.zero)
                annotateAvoidObstacle (minTimeToCollision * Speed);

            return avoidance;
        }


        // this version avoids all of the obstacles in an ObstacleGroup
        //
        // XXX 9-12-03: note this does NOT use the Obstacle::steerToAvoid protocol
        // XXX like the older steerToAvoidObstacle does/did.  It needs to be fixed
       
        public Vector3 steerToAvoidObstacles ( float minTimeToCollision, ArrayList obstacles)
        {
            Vector3 avoidance = new Vector3() ;

            if (obstacles == null)
            {
                return avoidance;
            }

            PathIntersection nearest;

            nearest = new PathIntersection(null);

            float minDistanceToCollision = minTimeToCollision * Speed;

            // test all obstacles for intersection with my forward axis,
            // select the one whose point of intersection is nearest


            
            //for (ObstacleIterator o = obstacles.begin(); o != obstacles.end(); o++)
            for (int i=0;i<obstacles.Count;i++)
            {
                SphericalObstacle o=(SphericalObstacle) obstacles[i];
                // xxx this should be a generic call on Obstacle, rather than
                // xxx this code which presumes the obstacle is spherical
                PathIntersection next = findNextIntersectionWithSphere (o);
                if (!nearest.intersect ||
                    (next.intersect &&
                     next.distance < nearest.distance))
                {
                    nearest = next;
                }
            }

            // when a nearest intersection was found
            if (nearest.intersect &&
                nearest.distance < minDistanceToCollision)
            {
                #if ANNOTATE_AVOIDOBSTACLES
                Debug.DrawLine(Position, nearest.obstacle.center, Color.red);
                Debug.Log("Avoiding obstacle at "+nearest.distance);
                #endif
                // show the corridor that was checked for collisions
                annotateAvoidObstacle (minDistanceToCollision);

                // compute avoidance steering force: take offset from obstacle to me,
                // take the component of that which is lateral (perpendicular to my
                // forward direction), set length to maxForce, add a bit of forward
                // component (in capture the flag, we never want to slow down)
                Vector3 offset = Position - nearest.obstacle.center;
                //avoidance = offset.perpendicularComponent (Forward);
                avoidance =  OpenSteerUtility.perpendicularComponent( offset,Forward);

                avoidance.Normalize();//.normalize ();
                avoidance *= MaxForce;
                avoidance += Forward * MaxForce * 0.75f;
            }

            return avoidance;
        }


        // ----------------------------------------------------------------------------
        // Unaligned collision avoidance behavior: avoid colliding with other nearby
        // vehicles moving in unconstrained directions.  Determine which (if any)
        // other other vehicle we would collide with first, then steers to avoid the
        // site of that potential collision.  Returns a steering force vector, which
        // is zero length if there is no impending collision.


       
        public Vector3 steerToAvoidNeighbors ( float minTimeToCollision, ArrayList others)
        {
            // first priority is to prevent immediate interpenetration
            Vector3 separation = steerToAvoidCloseNeighbors (0, others);
            if (separation != Vector3.zero) return separation;

            // otherwise, go on to consider potential future collisions
            float steer = 0;
            Vehicle threat = null;

            // Time (in seconds) until the most immediate collision threat found
            // so far.  Initial value is a threshold: don't look more than this
            // many frames into the future.
            float minTime = minTimeToCollision;

            // xxx solely for annotation
            Vector3 xxxThreatPositionAtNearestApproach=new Vector3();
            Vector3 xxxOurPositionAtNearestApproach = new Vector3();

            // for each of the other vehicles, determine which (if any)
            // pose the most immediate threat of collision.
            //for (AVIterator i = others.begin(); i != others.end(); i++)
            for (int i=0;i<others.Count;i++)
            {
                Vehicle other = (Vehicle) others[i];
                if (other != this)
                {	
                    // avoid when future positions are this close (or less)
                     float collisionDangerThreshold = Radius * 2;

                    // predicted time until nearest approach of "this" and "other"
                     float time = predictNearestApproachTime (other);

                    // If the time is in the future, sooner than any other
                    // threatened collision...
                    if ((time >= 0) && (time < minTime))
                    {
                        // if the two will be close enough to collide,
                        // make a note of it
                        if (computeNearestApproachPositions (other, time)
                            < collisionDangerThreshold)
                        {
                            minTime = time;
                            threat = other;
                            xxxThreatPositionAtNearestApproach
                                = hisPositionAtNearestApproach;
                            xxxOurPositionAtNearestApproach
                                = ourPositionAtNearestApproach;
                        }
                    }
                }
            }

            // if a potential collision was found, compute steering to avoid
            if (threat != null)
            {
                // parallel: +1, perpendicular: 0, anti-parallel: -1
                float parallelness = Vector3.Dot(Forward, threat.Forward);
                float angle = 0.707f;

                if (parallelness < -angle)
                {
                    // anti-parallel "head on" paths:
                    // steer away from future threat position
                    Vector3 offset = xxxThreatPositionAtNearestApproach - Position;
                    float sideDot = Vector3.Dot(offset, Side);
                    steer = (sideDot > 0) ? -1.0f : 1.0f;
                }
                else
                {
                    if (parallelness > angle)
                    {
                        // parallel paths: steer away from threat
                        Vector3 offset = threat.Position - Position;
                        float sideDot = Vector3.Dot(offset, Side);
                        steer = (sideDot > 0) ? -1.0f : 1.0f;
                    }
                    else
                    {
                        // perpendicular paths: steer behind threat
                        // (only the slower of the two does this)
                        if (threat.Speed <= Speed)
                        {
                            float sideDot = Vector3.Dot(Side, threat.Velocity);
                            steer = (sideDot > 0) ? -1.0f : 1.0f;
                        }
                    }
                }
               
                annotateAvoidNeighbor (threat,
                                       steer,
                                       xxxOurPositionAtNearestApproach,
                                       xxxThreatPositionAtNearestApproach);
            }

            return Side * steer;
        }



        // Given two vehicles, based on their current positions and velocities,
        // determine the time until nearest approach
        //
        // XXX should this return zero if they are already in contact?

       
        float predictNearestApproachTime (Vehicle other)
        {
            // imagine we are at the origin with no velocity,
            // compute the relative velocity of the other vehicle
            Vector3 myVelocity = Velocity;
            Vector3 otherVelocity = other.Velocity;
            Vector3 relVelocity = otherVelocity - myVelocity;
            float relSpeed = relVelocity.magnitude;

            // for parallel paths, the vehicles will always be at the same distance,
            // so return 0 (aka "now") since "there is no time like the present"
            if (relSpeed == 0) return 0;

            // Now consider the path of the other vehicle in this relative
            // space, a line defined by the relative position and velocity.
            // The distance from the origin (our vehicle) to that line is
            // the nearest approach.

            // Take the unit tangent along the other vehicle's path
            Vector3 relTangent = relVelocity / relSpeed;

            // find distance from its path to origin (compute offset from
            // other to us, find length of projection onto path)
            Vector3 relPosition = Position - other.Position;
            float projection = Vector3.Dot(relTangent, relPosition);

            return projection / relSpeed;
        }


        // Given the time until nearest approach (predictNearestApproachTime)
        // determine position of each vehicle at that time, and the distance
        // between them


       
        float computeNearestApproachPositions (Vehicle other, float time)
        {
            Vector3    myTravel =       Forward *       Speed * time;
            Vector3 otherTravel = other.Forward * other.Speed * time;

            Vector3    myFinal =       Position  +    myTravel;
            Vector3 otherFinal = other.Position  + otherTravel;

            // xxx for annotation
            ourPositionAtNearestApproach = myFinal;
            hisPositionAtNearestApproach = otherFinal;

            return (myFinal - otherFinal).magnitude;//Vector3::distance (myFinal, otherFinal);
        }



        // ----------------------------------------------------------------------------
        // avoidance of "close neighbors" -- used only by steerToAvoidNeighbors
        //
        // XXX  Does a hard steer away from any other agent who comes withing a
        // XXX  critical distance.  Ideally this should be replaced with a call
        // XXX  to steerForSeparation.


       
        public Vector3 steerToAvoidCloseNeighbors ( float minSeparationDistance, ArrayList others)
        {
            // for each of the other vehicles...
            //for (AVIterator i = others.begin(); i != others.end(); i++)    
            for (int i=0;i<others.Count;i++)
            {
                Vehicle other = (Vehicle) others[i];
                if (other != this)
                {
                     float sumOfRadii = Radius + other.Radius;
                     float minCenterToCenter = minSeparationDistance + sumOfRadii;
                     Vector3 offset = other.Position - Position;
                     float currentDistance = offset.magnitude;

                    if (currentDistance < minCenterToCenter)
                    {
                        annotateAvoidCloseNeighbor (other, minSeparationDistance);
                        return OpenSteerUtility.perpendicularComponent(-offset,Forward);
                    }
                }
            }
            
            // otherwise return zero
            return Vector3.zero;
        }


        // ----------------------------------------------------------------------------
        // used by boid behaviors: is a given vehicle within this boid's neighborhood?


       
        bool inBoidNeighborhood ( Vehicle other, float minDistance, float maxDistance, float cosMaxAngle)
        {
            if (other == this)
            {
                return false;
            }
            else
            {
                Vector3 offset = other.Position - Position;
                float distanceSquared = offset.sqrMagnitude;

                // definitely in neighborhood if inside minDistance sphere
                if (distanceSquared < (minDistance * minDistance))
                {
                    return true;
                }
                else
                {
                    // definitely not in neighborhood if outside maxDistance sphere
                    if (distanceSquared > (maxDistance * maxDistance))
                    {
                        return false;
                    }
                    else
                    {
                        // otherwise, test angular offset from forward axis
                         Vector3 unitOffset = offset / (float) System.Math.Sqrt (distanceSquared);
                         float forwardness = Vector3.Dot(Forward, unitOffset);
                        return forwardness > cosMaxAngle;
                    }
                }
            }
        }


        // ----------------------------------------------------------------------------
        // Separation behavior: steer away from neighbors
        public Vector3 steerForSeparation (float maxDistance, float cosMaxAngle, ArrayList flock)
        {
            // steering accumulator and count of neighbors, both initially zero
            Vector3 steering=Vector3.zero;
            int neighbors = 0;

            // for each of the other vehicles...
            //for (AVIterator other = flock.begin(); other != flock.end(); other++)
            for (int i = 0; i < flock.Count; i++)
            {
                Vehicle other = (Vehicle)flock[i];
                if (inBoidNeighborhood (other, Radius*3, maxDistance, cosMaxAngle))
                {
                    // add in steering contribution
                    // (opposite of the offset direction, divided once by distance
                    // to normalize, divided another time to get 1/d falloff)
                     Vector3 offset = (other).Position - Position;
                     float distanceSquared = Vector3.Dot(offset, offset);
                    steering += (offset / -distanceSquared);

                    // count neighbors
                    neighbors++;
                }
            }

            // divide by neighbors, then normalize to pure direction
            if (neighbors > 0) {
                steering = (steering / (float)neighbors);
                steering.Normalize();
            }

            return steering;
        }


        // ----------------------------------------------------------------------------
        // Alignment behavior: steer to head in same direction as neighbors


       
        public Vector3 steerForAlignment ( float maxDistance, float cosMaxAngle, ArrayList flock)
        {
            // steering accumulator and count of neighbors, both initially zero
            Vector3 steering=Vector3.zero;
            int neighbors = 0;

            // for each of the other vehicles...
            //for (AVIterator other = flock.begin(); other != flock.end(); other++)
            for (int i=0;i<flock.Count;i++)
            {
                Vehicle other = (Vehicle) flock[i];

                if (inBoidNeighborhood (other, Radius*3, maxDistance, cosMaxAngle))
                {
                    // accumulate sum of neighbor's heading
                    steering += other.Forward;

                    // count neighbors
                    neighbors++;
                }
            }

            // divide by neighbors, subtract off current heading to get error-
            // correcting direction, then normalize to pure direction
            if (neighbors > 0)
            {
                steering = ((steering / (float)neighbors) - Forward);
                steering.Normalize();
            }

            return steering;
        }


        // ----------------------------------------------------------------------------
        // Cohesion behavior: to to move toward center of neighbors



       
        public Vector3 steerForCohesion ( float maxDistance,  float cosMaxAngle, ArrayList flock)
        {
            // steering accumulator and count of neighbors, both initially zero
            Vector3 steering=Vector3.zero;
            int neighbors = 0;

            // for each of the other vehicles...
           // for (AVIterator other = flock.begin(); other != flock.end(); other++)
            for (int i = 0; i < flock.Count; i++)
            {
                Vehicle other = (Vehicle)flock[i];

                if (inBoidNeighborhood (other, Radius*3, maxDistance, cosMaxAngle))
                {
                    // accumulate sum of neighbor's positions
                    steering += other.Position;

                    // count neighbors
                    neighbors++;
                }
            }

            // divide by neighbors, subtract off current position to get error-
            // correcting direction, then normalize to pure direction
            if (neighbors > 0)
            {
                steering = ((steering / (float)neighbors) - Position);
                steering.Normalize();
            }

            return steering;
        }


        // ----------------------------------------------------------------------------
        // pursuit of another vehicle (& version with ceiling on prediction time)


       
        public Vector3 steerForPursuit (Vehicle quarry)
        {
            return steerForPursuit (quarry, float.MaxValue);
        }


       
        public Vector3 steerForPursuit (Vehicle quarry, float maxPredictionTime)
        {
            // offset from this to quarry, that distance, unit vector toward quarry
            Vector3 offset = quarry.Position - Position;
            float distance = offset.magnitude;
            Vector3 unitOffset = offset / distance;

            // how parallel are the paths of "this" and the quarry
            // (1 means parallel, 0 is pependicular, -1 is anti-parallel)
            float parallelness = Vector3.Dot(Forward, quarry.Forward);

            // how "forward" is the direction to the quarry
            // (1 means dead ahead, 0 is directly to the side, -1 is straight back)
            float forwardness = Vector3.Dot(Forward, unitOffset);

            float directTravelTime = distance / Speed;
            int f = intervalComparison (forwardness,  -0.707f, 0.707f);
            int p = intervalComparison (parallelness, -0.707f, 0.707f);

            float timeFactor = 0;       // to be filled in below
            Color color = Color.black;  // to be filled in below (xxx just for debugging)

            // Break the pursuit into nine cases, the cross product of the
            // quarry being [ahead, aside, or behind] us and heading
            // [parallel, perpendicular, or anti-parallel] to us.
            switch (f)
            {
                case +1:
                    switch (p)
                    {
                    case +1:          // ahead, parallel
                        timeFactor = 4;
                        color = Color.black;
                        break;
                    case 0:           // ahead, perpendicular
                        timeFactor = 1.8f;
                        color = Color.gray;
                        break;
                    case -1:          // ahead, anti-parallel
                        timeFactor = 0.85f;
                        color = Color.white;
                        break;
                    }
                    break;
                case 0:
                    switch (p)
                    {
                    case +1:          // aside, parallel
                        timeFactor = 1;
                        color = Color.red;
                        break;
                    case 0:           // aside, perpendicular
                        timeFactor = 0.8f;
                        color = Color.yellow;
                        break;
                    case -1:          // aside, anti-parallel
                        timeFactor = 4;
                        color = Color.green;
                        break;
                    }
                    break;
                case -1:
                    switch (p)
                    {
                    case +1:          // behind, parallel
                        timeFactor = 0.5f;
                        color = Color.cyan;
                        break;
                    case 0:           // behind, perpendicular
                        timeFactor = 2;
                        color = Color.blue;
                        break;
                    case -1:          // behind, anti-parallel
                        timeFactor = 2;
                        color = Color.magenta;
                        break;
                    }
                    break;
            }

            // estimated time until intercept of quarry
            float et = directTravelTime * timeFactor;

            // xxx experiment, if kept, this limit should be an argument
            float etl = (et > maxPredictionTime) ? maxPredictionTime : et;

            // estimated position of quarry at intercept
            Vector3 target = quarry.predictFuturePosition (etl);

            // annotation
            #if DEBUG
            annotationLine (Position,
                            target,
                            gaudyPursuitAnnotation ? color : Color.gray);
            #endif

            return steerForSeek (target);
        }

        // ----------------------------------------------------------------------------
        // evasion of another vehicle


       
        public Vector3 steerForEvasion ( Vehicle menace, float maxPredictionTime)
        {
            // offset from this to menace, that distance, unit vector toward menace
            Vector3 offset = menace.Position - Position;
            float distance = offset.magnitude;

            float roughTime = distance / menace.Speed;
            float predictionTime = ((roughTime > maxPredictionTime) ?
                                          maxPredictionTime :
                                          roughTime);

            Vector3 target = menace.predictFuturePosition (predictionTime);

            return steerForFlee (target);
        }
       

        // ----------------------------------------------------------------------------
        // tries to maintain a given speed, returns a maxForce-clipped steering
        // force along the forward/backward axis


       
        public Vector3 steerForTargetSpeed ( float targetSpeed)
        {
             float mf = MaxForce;
             float speedError = targetSpeed - Speed;
             return Forward * Mathf.Clamp (speedError, -mf, +mf);
        }


        // ----------------------------------------------------------------------------
        // xxx experiment cwr 9-6-02


       
        public PathIntersection findNextIntersectionWithSphere (SphericalObstacle obs)
        {
            // xxx"SphericalObstacle& obs" should be "const SphericalObstacle&
            // obs" but then it won't let me store a pointer to in inside the
            // PathIntersection

            // This routine is based on the Paul Bourke's derivation in:
            //   Intersection of a Line and a Sphere (or circle)
            //   http://www.swin.edu.au/astronomy/pbourke/geometry/sphereline/

            float b, c, d, p, q, s;
            Vector3 lc;

            // initialize pathIntersection object
            PathIntersection intersection = new PathIntersection(obs);

            // find "local center" (lc) of sphere in boid's coordinate space
            lc = LocalizePosition (obs.center);

            // computer line-sphere intersection parameters
            b = -2 * lc.z;
            c = square (lc.x) + square (lc.y) + square (lc.z) - 
                square (obs.radius + Radius);
            d = (b * b) - (4 * c);

            // when the path does not intersect the sphere
            if (d < 0) return intersection;

            // otherwise, the path intersects the sphere in two points with
            // parametric coordinates of "p" and "q".
            // (If "d" is zero the two points are coincident, the path is tangent)
            s = (float) System.Math.Sqrt(d);
            p = (-b + s) / 2;
            q = (-b - s) / 2;

            // both intersections are behind us, so no potential collisions
            if ((p < 0) && (q < 0)) return intersection; 

            // at least one intersection is in front of us
            intersection.intersect = true;
            intersection.distance =
                ((p > 0) && (q > 0)) ?
                // both intersections are in front of us, find nearest one
                ((p < q) ? p : q) :
                // otherwise only one intersections is in front, select it
                ((p > 0) ? p : q);
            return intersection;
        }

        public float scalarRandomWalk ( float initial, float walkspeed, float min, float max)
        {
            float next = initial + (((frandom01() * 2) - 1) * walkspeed);
            if (next < min) return min;
            if (next > max) return max;
            return next;
        }


        // From utility

        public float frandom01 ()
        {
            return ((float) randomGenerator.NextDouble());//((float) rand ()) / ((float) RAND_MAX));
        }

        public Vector3 truncateLength(Vector3 tVector, float maxLength)
        {
            float tLength = tVector.magnitude;
            Vector3 returnVector = tVector;
            if (tLength > maxLength)
            {
                returnVector.Normalize();// = tVector - tVector*(tLength - maxLength);
                returnVector *= maxLength;
            }
            return returnVector;
        }

        public int intervalComparison (float x, float lowerBound, float upperBound)
        {
            if (x < lowerBound) return -1;
            if (x > upperBound) return +1;
            return 0;
        }

        public float square (float x)
        {
            return x * x;
        }

        public virtual void annotationLine(Vector3 startPoint, Vector3 endPoint, Color color)
        {
            Debug.DrawLine(startPoint, endPoint, color);
        }
    }
}
