// ----------------------------------------------------------------------------
//
// Ported to Unity by Ricardo J. Mendez http://www.arges-systems.com/
//
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
    public struct PathRelativePosition
    {
        public float outside;
        public Vector3 tangent;
		public int segmentIndex;
	}

    public abstract class Pathway : IPathway
    {
        private bool _isCyclic;
        
        public bool IsCyclic
        {
            get
            {
                return _isCyclic;
            }
			protected set 
			{
				_isCyclic = value;
			}
        }
        
        
        public float TotalPathLength
        {
            get
            {
                return GetTotalPathLength();
            }
        }
        
        public Vector3 FirstPoint
        {
            get
            {
                return GetFirstPoint();
            }
        }
        
        
        public Vector3 LastPoint
        {
            get
            {
                return GetLastPoint();
            }
        }
		
		public int SegmentCount
		{
			get
			{
				return GetSegmentCount();
			}
		}
        
        
        // Returns the total path length. It's expected to be overriden by the 
        // actual implementations.
        protected virtual float GetTotalPathLength()
        {
            return 0;
        }

        /// <summary>
        /// Returns the first point along the path
        /// </summary>
        /// <remarks>
        /// Descendents are expected to override this method
        /// </remarks>
        protected virtual Vector3 GetFirstPoint()
        {
            return Vector3.zero;
        }
                
        /// <summary>
        /// Returns the last point along the path
        /// </summary>
        /// <remarks>
        /// Descendents are expected to override this method
        /// </remarks>
        protected virtual Vector3 GetLastPoint()
        {
            return Vector3.zero;
        }
		
		protected virtual int GetSegmentCount()
		{
			return 0;
		}
        
        // Given an arbitrary point ("A"), returns the nearest point ("P") on
        // this path.  Also returns, via output arguments, the path tangent at
        // P and a measure of how far A is outside the Pathway's "tube".  Note
        // that a negative distance indicates A is inside the Pathway.
        public virtual Vector3 MapPointToPath(Vector3 point, ref PathRelativePosition tStruct) { return Vector3.zero; }

        // given a distance along the path, convert it to a point on the path
        public virtual Vector3 MapPathDistanceToPoint(float pathDistance) { return Vector3.zero; }

        // Given an arbitrary point, convert it to a distance along the path.
        public virtual float MapPointToPathDistance(Vector3 point) { return 0; }

        // is the given point inside the path tube?
        public bool IsInsidePath(Vector3 point)
        {
            var tStruct = new PathRelativePosition();

            MapPointToPath(point, ref tStruct);
            return tStruct.outside < 0;
        }

        // how far outside path tube is the given point?  (negative is inside)
        public float HowFarOutsidePath(Vector3 point)
        {
            var tStruct = new PathRelativePosition();

            MapPointToPath(point, ref tStruct);
            return tStruct.outside;
        }
		
		
		public virtual void DrawGizmos()
		{
		}
    }
}
