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
		public SteerLibrary( Vector3 position, float mass ) : base( position, mass ){}
		public SteerLibrary( Transform transform, float mass ) : base( transform, mass ){}
		public SteerLibrary( Rigidbody rigidbody ) : base( rigidbody ){}

		// reset state
		public void resetSteering ()
		{
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

			return avoidance;
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

	}
}
