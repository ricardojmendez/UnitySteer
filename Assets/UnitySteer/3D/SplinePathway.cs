// ----------------------------------------------------------------------------
//
// Written by Ricardo J. Mendez http://www.arges-systems.com/ based on 
// OpenSteer's PolylinePathway.
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

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
        private Vector3[] _splineNodes;

        /// <summary>
        /// Number of segments to use when drawing the spline
        /// </summary>
        [SerializeField] private int _pathDrawResolution = 50;

        public SplinePathway()
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
        public SplinePathway(IList<Vector3> path, float radius) : base(path, radius)
        {
        }

        protected override void PrecalculatePathData()
        {
            base.PrecalculatePathData();
            // Place the two control nodes
            var splineNodeLength = Path.Count + 2;
            _splineNodes = new Vector3[splineNodeLength];
            _splineNodes[0] = Path[0] - Normals[1] * 2;
            for (var i = 0; i < Path.Count; i++)
            {
                _splineNodes[i + 1] = Path[i];
            }
            _splineNodes[splineNodeLength - 1] = Path.Last() + Normals.Last() * 4;
        }

        /// <summary>
        /// Given an arbitrary point ("A"), returns the nearest point ("P") on
        /// this path.  Also returns, via output arguments, the path Tangent at
        /// P and a measure of how far A is Outside the Pathway's "tube".  Note
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
            var splinePoint = CalculateCatmullRomPoint(1, distance);

            // return point on path
            return splinePoint;
        }

        public override Vector3 MapPathDistanceToPoint(float pathDistance)
        {
            if (_splineNodes.Length < 5)
            {
                return base.MapPathDistanceToPoint(pathDistance);
            }

            pathDistance = Mathf.Min(TotalPathLength, pathDistance);

            var nodeForDistance = 0;
            var lastTotal = 0f;
            var totalLength = 0f;
            // We skip the first node because its length will always be zero,
            // and besides weĺl pass this to GetPathPoint which has one extra
            // node
            for (var i = 1; i < Lengths.Count && nodeForDistance == 0; i++)
            {
                lastTotal = totalLength;
                totalLength += Lengths[i];
                if (totalLength >= pathDistance)
                {
                    nodeForDistance = i;
                }
            }

            var segmentLength = Lengths[nodeForDistance];
            var remainingLength = pathDistance - lastTotal;
            var pctComplete = Mathf.Approximately(segmentLength, 0) ? 1 : (remainingLength / segmentLength);

            return CalculateCatmullRomPoint(nodeForDistance, pctComplete);
        }

        /// <summary>
        /// Calculates a catmull-rom point for a spline defined by a set of spline nodes,
        /// based on a current node index
        /// </summary>
        /// <returns>The catmull rom point.</returns>
        /// <param name="currentNode">Current node. Index 0 is the initial control point, index 1 the first actual path node.</param>
        /// <param name="percentComplete">Percent complete for this segment.</param>
        private Vector3 CalculateCatmullRomPoint(int currentNode, float percentComplete)
        {
            var percentCompleteSquared = percentComplete * percentComplete;
            var percentCompleteCubed = percentCompleteSquared * percentComplete;

            var start = _splineNodes[currentNode];
            var end = _splineNodes[currentNode + 1];
            var previous = _splineNodes[currentNode - 1];
            var next = _splineNodes[currentNode + 2];

            return previous * (-0.5f * percentCompleteCubed + percentCompleteSquared - 0.5f * percentComplete) +
                   start * (1.5f * percentCompleteCubed - 2.5f * percentCompleteSquared + 1.0f) +
                   end * (-1.5f * percentCompleteCubed + 2.0f * percentCompleteSquared + 0.5f * percentComplete) +
                   next * (0.5f * percentCompleteCubed - 0.5f * percentCompleteSquared);
        }

        public override void DrawGizmos()
        {
            Debug.DrawLine(_splineNodes[0], Path[0], Color.gray);
            var lastPosition = Path[0];
            for (var i = 0; i < Path.Count - 1; i++)
            {
                for (var segment = 0; segment < _pathDrawResolution; segment++)
                {
                    var nextPosition = CalculateCatmullRomPoint(i + 1, segment / (float) _pathDrawResolution);
                    Debug.DrawLine(lastPosition, nextPosition, Color.green);
                    lastPosition = nextPosition;
                }
            }
            Debug.DrawLine(lastPosition, _splineNodes.Last(), Color.gray);
        }
    }
}