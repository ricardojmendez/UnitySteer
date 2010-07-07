// ----------------------------------------------------------------------------
//
// Ported to Unity by Ricardo J. Mendez - http://www.arges-systems.com/
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
	public class SteeringVehicle
	{
		private Transform transform;
		private Rigidbody rigidbody;
		private float internalMass, radius, speed, maxSpeed, maxForce;
		private bool movesVertically = true;
		private Vector3 internalPosition, internalSide, internalForward, internalUp;
		

		public SteeringVehicle( Vector3 position, float mass )
		{
			this.Position = position;
			internalMass = mass;
		}		
		
		public SteeringVehicle( Transform transform, float mass )
		{
			this.transform = transform;
			internalMass = mass;
		}
		
		
		public SteeringVehicle( Rigidbody rigidbody )
		{
			this.rigidbody = rigidbody;
		}
		
		
		public Vector3 Position
		{
			get
			{
				if( rigidbody != null )
				{
					return rigidbody.position;
				}
				if (transform != null)
				{
					return transform.position;
				}
				return internalPosition;
			}
			set
			{
				if( !MovesVertically )
				{
					value.y = Position.y;
				}
				if( rigidbody != null )
				{
					rigidbody.MovePosition( value );
					return;
				}
				else if (transform != null)
				{
					transform.position = value;
				}
				else
				{
				    internalPosition = value;
				}
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
				if (transform != null)
				{
					return transform.forward;
				}
				return internalForward;
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
				}
				else if (transform != null)
				{
					transform.forward = value;
				}
				else 
				{
					internalForward = value;
					RecalculateSide();
				}
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
				else if (transform != null)
				{
					return transform.right;
				}
				else
					return internalSide;
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
				if (transform != null)
				{
					return transform.up;
				}
				return internalUp;
			}
			set
			{
				if( rigidbody != null )
				{
					rigidbody.transform.up = value;
				}
				else if (transform != null)
				{
					transform.up = value;
				}
				else
				{
					internalUp = value;
					RecalculateSide();
				}
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
				return internalMass;
			}
			set
			{
				if( rigidbody != null )
				{
					rigidbody.mass = value;
				}
				else
				{
				    internalMass = value;
				}
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
		
		protected GameObject GameObject
		{
			get
			{
				GameObject go = (rigidbody != null ) ? rigidbody.gameObject : transform.gameObject;
				return go;
			}
		}
		
		public virtual Vector3 predictFuturePosition(float predictionTime) { return Vector3.zero; }

		private void RecalculateSide()
		{
			internalSide = Vector3.Cross(internalForward, internalUp);
			internalSide.Normalize();
		}
	}
}
 