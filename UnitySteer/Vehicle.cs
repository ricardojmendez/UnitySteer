// ----------------------------------------------------------------------------
//
// Ported to Unity by Ricardo J. Méndez - http://www.arges-systems.com/
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

using System;
using System.Collections;
using System.Text;
using UnityEngine;

namespace UnitySteer
{
	// TODO: Alter use of this and SimpleVehicle to apply forces when using Rigidbody in stead of MovePosition
    public class Vehicle
    {
        ArrayList neighbors = new ArrayList();

        
		private Transform transform;
		private Rigidbody rigidbody;
		private float transformMass, radius, speed, maxSpeed, maxForce;
		private bool movesVertically;
		
		
		
		public Vehicle( Transform transform, float mass )
		{
			this.transform = transform;
			transformMass = mass;
		}
		
		
		
		public Vehicle( Rigidbody rigidbody )
		{
			this.rigidbody = rigidbody;
		}
		
		public ArrayList Neighbors
        {
            get
            {
                return neighbors;
            }
        }
		
		public Vector3 Position
		{
			get
			{
				if( rigidbody != null )
				{
					return rigidbody.position;
				}
				return transform.position;
			}
			set
			{
				if( !MovesVertically )
				{
					value = new Vector3( value.x, Position.y, value.z );
				}
				
				if( rigidbody != null )
				{
					rigidbody.MovePosition( value );
					return;
				}
				transform.position = value;
			}
		}
		
		
		
		public Vector3 Forward
		{
			get
			{
				if( rigidbody != null )
				{
					return rigidbody.transform.forward;
				}
				
				return transform.forward;
			}
			set
			{
				if( !MovesVertically )
				{
					value = new Vector3( value.x, Forward.y, value.z );
				}
				
				if( rigidbody != null )
				{
					rigidbody.transform.forward = value;
					return;
				}
				
				transform.forward = value;
			}
		}
		
		
		
		public Vector3 Side
		{
			get
			{
				if( rigidbody != null )
				{
					return rigidbody.transform.right;
				}
				
				return transform.right;
			}
		}
		
		
		
		public Vector3 Up
		{
			get
			{
				if( rigidbody != null )
				{
					return rigidbody.transform.up;
				}
				
				return transform.up;
			}
			set
			{
				if( rigidbody != null )
				{
					rigidbody.transform.up = value;
					return;
				}
				
				transform.up = value;
			}
		}
		
		
		
		public float Mass
		{
			get
			{
				if( rigidbody != null )
				{
					return rigidbody.mass;
				}
				return transformMass;
			}
			set
			{
				if( rigidbody != null )
				{
					rigidbody.mass = value;
					return;
				}
				transformMass = value;
			}
		}



		public float Speed
		{
			get
			{
				return speed;
			}
			set
			{
				speed = value;
			}
		}
		
		
		
		public float MaxSpeed
		{
			get
			{
				return maxSpeed;
			}
			set
			{
				maxSpeed = value;
			}
		}
		
		
		
		public float MaxForce
		{
			get
			{
				return maxForce;
			}
			set
			{
				maxForce = value;
			}
		}
		
		
		
		public bool MovesVertically
		{
			get
			{
				return movesVertically;
			}
			set
			{
				movesVertically = value;
			}
		}



		public Vector3 Velocity
		{
			get
			{
				return Forward * speed;
			}
		}
		
		
		
		public float Radius
		{
			get
			{
				return radius;
			}
			set
			{
				radius = value;
			}
		}
		
		
		protected Transform Transform
		{
		    get
		    {
	            Transform t = (rigidbody != null ) ? rigidbody.transform : transform;
	            return t;
    	    }
		}
		
		
        public virtual Vector3 predictFuturePosition(float predictionTime) { return Vector3.zero; }


		// TODO: NOTE: What the heck is the point of this? We wants?
        public void ResetLocalSpace()
        {
			if( rigidbody != null )
			{
				rigidbody.transform.up = new Vector3(0, 1, 0);
			}
			else
			{
				transform.up = new Vector3(0, 1, 0);
			}
	
			if( rigidbody != null )
			{
				rigidbody.transform.forward = new Vector3(0, 0, 1);
			}
			else
			{
				transform.forward = new Vector3(0, 0, 1);
			}

            Position = new Vector3(0, 0, 0);
        }

        // ------------------------------------------------------------------------
        // transform a point in global space to its equivalent in local space
        public Vector3 LocalizePosition(Vector3 globalPosition)
        {
            return Transform.InverseTransformPoint(globalPosition);
        }

    }
}
