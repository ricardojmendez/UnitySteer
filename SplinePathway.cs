//#define USE_GOKIT
// ----------------------------------------------------------------------------
//
// Written by Ricardo J. Mendez http://www.arges-systems.com/ based on 
// OpenSteer's PolylinePathway.  Contains code from GoKit's Catmull-Rom
// spline solver (https://github.com/prime31/GoKit)
//
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
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace UnitySteer
{
    /// <summary>
    /// Represents a Pathway created from a list of Vector3s, interpreted
    /// as a spline.  
    /// 
    /// It uses a trivial approach - if the path has less than three nodes,
    /// it will behave the same as a regular Vector3Pathway.
    /// </summary>
    /// <remarks>
    /// Chances are this is not what you want to use to create a pathway for
    /// bipeds dealing with spatial constraints (say, following a navmesh).
    /// I'm writing it to get smoother turning on a group of flying agents.
    /// </remarks>
    public class SplinePathway : Vector3Pathway
    {
        Vector3[] _splineNodes;
        
        public SplinePathway(): base()
        {
        }
        
        
        /// <summary>
        /// Constructs a Vector3Pathway given an array of points and a path radius
        /// </summary>
        /// <param name="path">
        /// List of Vector3 to be used for the path in world space <see cref="Vector3"/>
        /// </param>
        /// <param name="radius">
        /// Radius to use for the connections <see cref="System.Single"/>
        /// </param>
        /// <remarks>The current implementation assumes that all pathways will 
        /// have the same radius.
        /// </remarks>
        public SplinePathway (IList<Vector3> path, float radius):base(path, radius)
        {
        }
        
        /// <summary>
        /// Constructs the Pathway from a list of Vector3
        /// </summary>
        /// <param name="path">
        /// A list of Vector3 defining the path points in world space<see cref="Vector3"/>
        /// </param>
        /// <param name="radius">
        /// Radius to use for the connections<see cref="System.Single"/>
        /// </param>
        public override void Initialize (IList<Vector3> path, float radius)
        {
            base.Initialize(path, radius);
            // Place the two control nodes
            _splineNodes = new Vector3[path.Count + 2];
            _splineNodes[0] = path.First() - Normals[1] * 2;
            for (int i = 0; i < path.Count; i++)
            {
                _splineNodes[i+1] = path[i];
            }
            _splineNodes[path.Count] = path.Last() + Normals.Last() * 2;
        }
        
        /// <summary>
        /// Given an arbitrary point ("A"), returns the nearest point ("P") on
        /// this path.  Also returns, via output arguments, the path tangent at
        /// P and a measure of how far A is outside the Pathway's "tube".  Note
        /// that a negative distance indicates A is inside the Pathway.
        /// </summary>
        /// <param name="point">Reference point.</param>
        /// <param name="pathRelative">Structure indicating the relative path position.</param>
        /// <returns>The closest point to the received reference point.</returns>
        public override Vector3 MapPointToPath(Vector3 point, ref PathRelativePosition pathRelative)
        {
            // Approximate the closest path point on a linear path
            var onPath = base.MapPointToPath(point, ref pathRelative);
            
            var distance = MapPointToPathDistance(onPath) / TotalPathLength;
            var splinePoint = GetPathPoint (distance);
            
            // return point on path
            return splinePoint;
        }
        
        public override Vector3 MapPathDistanceToPoint(float pathDistance)
        {
            return (_splineNodes.Length < 5) ? base.MapPathDistanceToPoint(pathDistance) : GetPathPoint(pathDistance / TotalPathLength);
        }
        
        public Vector3 GetPathPoint(float t)
        {
            int numSections = _splineNodes.Length - 3;
            int currentNode = Mathf.Min( Mathf.FloorToInt( t * (float)numSections ), numSections - 1 );
            float u = t * numSections - currentNode;
            
            Vector3 a = _splineNodes[currentNode];
            Vector3 b = _splineNodes[currentNode + 1];
            Vector3 c = _splineNodes[currentNode + 2];
            Vector3 d = _splineNodes[currentNode + 3];
            
            return .5f *
                (
                    ( -a + 3f * b - 3f * c + d ) * ( u * u * u )
                    + ( 2f * a - 5f * b + 4f * c - d ) * ( u * u )
                    + ( -a + c ) * u
                    + 2f * b
                    );
        }

#if USE_GOKIT
        public override void DrawGizmos()
        {
            GoKit.GoSpline.drawGizmos(_splineNodes, 50);
            base.DrawGizmos();
        }
#endif
    }
}