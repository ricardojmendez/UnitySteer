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
		public bool IsCyclic { get; protected set; }
        
		public abstract float TotalPathLength { get; }
        
		public abstract Vector3 FirstPoint { get; }
        
		public abstract Vector3 LastPoint { get; }

		public float Radius  { get; set; }
				
		public abstract int SegmentCount { get; }
	    
        
        /// <summary>
        /// Given an arbitrary point ("A"), returns the nearest point ("P") on
        /// this path.  Subclasses will also fill in extra information about the
        /// relative path position on the pathRelative structure.
        /// </summary>
        /// <param name="point">Reference point.</param>
        /// <param name="pathRelative">Structure indicating the relative path position.</param>
        /// <returns>The closest point to the received reference point.</returns>
        public abstract Vector3 MapPointToPath(Vector3 point, ref PathRelativePosition pathRelative);

        /// <summary>
        /// Given a distance along the path, convert it to a specific point on the path.
        /// </summary>
        /// <param name="pathDistance">Path distance to calculate corresponding point for.</param>
        /// <returns>The corresponding path point to the path distance.</returns>
        public abstract Vector3 MapPathDistanceToPoint(float pathDistance);

        // Given an arbitrary point, convert it to a distance along the path.
        /// <summary>
        /// Maps the reference point to a distance along the path.
        /// </summary>
        /// <param name="point">Reference point.</param>
        /// <returns>The distance along the path for the point.</returns>
        public abstract float MapPointToPathDistance(Vector3 point);

        /// <summary>
        /// Determines whether the received point is inside the path.
        /// </summary>
        /// <param name="point">Point to evaluate.</param>
        /// <returns><c>true</c> if the point is inside the path; otherwise, <c>false</c>.</returns>
        public bool IsInsidePath(Vector3 point)
        {
            var tStruct = new PathRelativePosition();

            MapPointToPath(point, ref tStruct);
            return tStruct.outside < 0;
        }

        /// <summary>
        /// Calculates how far outside the path is the reference point.
        /// </summary>
        /// <param name="point">Reference point.</param>
        /// <returns>How far outside the path is the reference point.</returns>
        public float HowFarOutsidePath(Vector3 point)
        {
            var tStruct = new PathRelativePosition();

            MapPointToPath(point, ref tStruct);
            return tStruct.outside;
        }
		
		
        public abstract void DrawGizmos();
    }
}
