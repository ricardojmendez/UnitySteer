// ----------------------------------------------------------------------------
//
// Ported to Unity by Ricardo J. Mendez http://www.arges-systems.com/
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
#define ANNOTATE_AVOIDNEIGHBORS
#define ANNOTATE_PATH

using System;
using System.Collections;
using System.Text;
using UnityEngine;


namespace UnitySteer
{
	public class SteerLibrary : SteeringVehicle
	{
		#if DEBUG
		bool gaudyPursuitAnnotation;
		#endif
		
		private Transform tether;
		private float	  maxDistance;
		private float	  maxDistanceSquared;
		
		private float	  avoidAngleCos = 0.707f;
		

		public SteerLibrary( Vector3 position, float mass ) : base( position, mass ){}
		public SteerLibrary( Transform transform, float mass ) : base( transform, mass ){}
		public SteerLibrary( Rigidbody rigidbody ) : base( rigidbody ){}
		// TODO: Consider doing a call to resetSteering() within these?
		

		// reset state
		public void resetSteering ()
		{
			// default to non-gaudyPursuitAnnotation
			#if DEBUG
			gaudyPursuitAnnotation = true;
			#endif
		}

		bool isAhead ( Vector3 target)	{return isAhead (target, 0.707f);}
		bool isAside ( Vector3 target)	{return isAside (target, 0.707f);}
		bool isBehind ( Vector3 target)	 {return isBehind (target, -0.707f);}

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
			Debug.DrawLine(Position, future, Color.white);
			Debug.DrawLine(Position, onPath, Color.yellow);
			Debug.DrawLine(Position, target, Color.magenta);
		}

		// called when steerToAvoidCloseNeighbors decides steering is required
		// (default action is to do nothing, layered classes can overload it)
		public virtual void annotateAvoidCloseNeighbor(SteeringVehicle otherVehicle, Vector3 component)
		{
			Debug.DrawLine(Position, otherVehicle.Position, Color.red);
			Debug.DrawRay (Position, component*3, Color.yellow);
		}

		// called when steerToAvoidNeighbors decides steering is required
		// (default action is to do nothing, layered classes can overload it)
		public virtual void annotateAvoidNeighbor (	 SteeringVehicle vehicle, float steer, Vector3 position, Vector3 threatPosition)
		{
			Debug.DrawLine(Position, vehicle.Position, Color.red); // Neighbor position
			Debug.DrawLine(Position, position, Color.green);	   // Position we're aiming for
		}

		public Vector3 steerForSeek(Vector3 target)
		{
			Vector3 desiredVelocity = target - Position;
			return desiredVelocity - Velocity;
		}


		// ----------------------------------------------------------------------------
		// Path Following behaviors

		public Vector3 steerToStayOnPath(float predictionTime, Pathway path)
		{
			// predict our future position
			Vector3 futurePosition = predictFuturePosition (predictionTime);

			// find the point on the path nearest the predicted future position
			mapReturnStruct tStruct = new mapReturnStruct();

			Vector3 onPath = path.mapPointToPath(futurePosition, ref tStruct);

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
				#if ANNOTATE_PATH
				annotatePathFollowing (futurePosition, onPath, onPath,tStruct.outside);
				#endif
				return steerForSeek (onPath);
			}
		}

		// ----------------------------------------------------------------------------
		// Obstacle Avoidance behavior
		//
		// Returns a steering force to avoid a given obstacle.	The purely lateral
		// steering force will turn our vehicle towards a silhouette edge of the
		// obstacle.  Avoidance is required when (1) the obstacle intersects the
		// vehicle's current path, (2) it is in front of the vehicle, and (3) is
		// within minTimeToCollision seconds of travel at the vehicle's current
		// velocity.  Returns a zero vector value (Vector3::zero) when no avoidance is
		// required.
		//
		// XXX The current (4-23-03) scheme is to dump all the work on the various
		// XXX Obstacle classes, making them provide a "steer vehicle to avoid me"
		// XXX method.	This may well change.
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


		// ----------------------------------------------------------------------------
		// Unaligned collision avoidance behavior: avoid colliding with other nearby
		// vehicles moving in unconstrained directions.	 Determine which (if any)
		// other other vehicle we would collide with first, then steers to avoid the
		// site of that potential collision.  Returns a steering force vector, which
		// is zero length if there is no impending collision.
		public Vector3 steerToAvoidNeighbors(float minTimeToCollision, ArrayList others)
		{
			/*
			// first priority is to prevent immediate interpenetration
			Vector3 separation = steerToAvoidCloseNeighbors (0, others);
			if (separation != Vector3.zero) 
			{
				return separation;
			}
			*/

			// otherwise, go on to consider potential future collisions
			float steer = 0;
			SteeringVehicle threat = null;

			// Time (in seconds) until the most immediate collision threat found
			// so far.	Initial value is a threshold: don't look more than this
			// many frames into the future.
			float minTime = minTimeToCollision;

			Vector3 threatPositionAtNearestApproach = Vector3.zero;
			Vector3 ourPositionAtNearestApproach = Vector3.zero;

			// for each of the other vehicles, determine which (if any)
			// pose the most immediate threat of collision.
			for (int i=0; i<others.Count; i++)
			{
				SteeringVehicle other = (SteeringVehicle) others[i];
				if (other != this)
				{	
					// avoid when future positions are this close (or less)
					float collisionDangerThreshold = Radius + other.Radius;

					// predicted time until nearest approach of "this" and "other"
					float time = predictNearestApproachTime (other);
					
					// If the time is in the future, sooner than any other
					// threatened collision...
					if ((time >= 0) && (time < minTime))
					{
						// if the two will be close enough to collide,
						// make a note of it
						Vector3 ourPos = Vector3.zero;
						Vector3 hisPos = Vector3.zero;
						float	dist   = computeNearestApproachPositions (other, time, ref ourPos, ref hisPos);
						
						if (dist < collisionDangerThreshold)
						{
							minTime = time;
							threat = other;
							threatPositionAtNearestApproach = hisPos;
							ourPositionAtNearestApproach = ourPos;
						}
					}
				}
			}

			// if a potential collision was found, compute steering to avoid
			if (threat != null)
			{
				// parallel: +1, perpendicular: 0, anti-parallel: -1
				float parallelness = Vector3.Dot(Forward, threat.Forward);
				// Debug.Log("Parallel "+parallelness + " "+avoidAngleCos+" "+threatPositionAtNearestApproach);

				if (parallelness < -avoidAngleCos)
				{
					// anti-parallel "head on" paths:
					// steer away from future threat position
					Vector3 offset = threatPositionAtNearestApproach - Position;
					float sideDot = Vector3.Dot(offset, Side);
					steer = (sideDot > 0) ? -1.0f : 1.0f;
				}
				else if (parallelness > avoidAngleCos)
				{
					// parallel paths: steer away from threat
					Vector3 offset = threat.Position - Position;
					float sideDot = Vector3.Dot(offset, Side);
					steer = (sideDot > 0) ? -1.0f : 1.0f;
				}
				else 
				{
					/* 
						Perpendicular paths: steer behind threat

						Only the slower vehicle attempts this, unless that 
						slower vehicle is static.  If both have the same
						speed, then roll the dice.						
						
						Something to test is making a slower vehicle fall
						behind, while a faster vehicle cuts ahead.
					 */
					if (Speed < threat.Speed
							 || threat.Speed == 0
							 || UnityEngine.Random.value <= 0.25f) 
					{
						float sideDot = Vector3.Dot(Side, threat.Velocity);
						steer = (sideDot > 0) ? -1.0f : 1.0f;
					}
				}
				
				/* Steer will end up being applied as a multiplier to the
				   vehicle's side vector. If we simply apply te -1/+1 being
				   assigned above, then we'll end up with a unit displacement
				   from the other object's position. We should account for
				   both its radius and our own.
				 */
				steer *= Radius + threat.Radius;

				#if ANNOTATE_AVOIDNEIGHBORS
				annotateAvoidNeighbor (threat,
									   steer,
									   ourPositionAtNearestApproach,
									   threatPositionAtNearestApproach);
				#endif
			}

			return Side * steer;
		}



		// Given two vehicles, based on their current positions and velocities,
		// determine the time until nearest approach
		//
		// XXX should this return zero if they are already in contact?

	   
		float predictNearestApproachTime (SteeringVehicle other)
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
		float computeNearestApproachPositions(SteeringVehicle other, float time, 
											  ref Vector3 ourPosition, 
											  ref Vector3 hisPosition)
		{
			Vector3	   myTravel =		Forward *		Speed * time;
			Vector3 otherTravel = other.Forward * other.Speed * time;

			ourPosition =		Position + myTravel;
			hisPosition = other.Position + otherTravel;

			return Vector3.Distance(ourPosition, hisPosition);
		}



		// ----------------------------------------------------------------------------
		// avoidance of "close neighbors" -- used only by steerToAvoidNeighbors
		//
		// XXX	Does a hard steer away from any other agent who comes withing a
		// XXX	critical distance.	Ideally this should be replaced with a call
		// XXX	to steerForSeparation.
		public Vector3 steerToAvoidCloseNeighbors (float minSeparationDistance, ArrayList others)
		{
			// for each of the other vehicles...
			Vector3 result = Vector3.zero;
			for (int i=0;i<others.Count;i++)
			{
				SteeringVehicle other = (SteeringVehicle) others[i];
				if (other != this)
				{
					 float sumOfRadii = Radius + other.Radius;
					 float minCenterToCenter = minSeparationDistance + sumOfRadii;
					 Vector3 offset = other.Position - Position;
					 float currentDistance = offset.magnitude;

					if (currentDistance < minCenterToCenter)
					{
						result = OpenSteerUtility.perpendicularComponent(-offset,Forward);
						
						#if ANNOTATE_AVOIDNEIGHBORS
						annotateAvoidCloseNeighbor (other, result);
						#endif
					}
				}
			}
			return result;;
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
	}
}
