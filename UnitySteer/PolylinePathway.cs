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
	
	/// <summary>
	/// Represents a pathway as a series of points
	/// </summary>
	/// <remarks>
    /// NOTE RJM: This class keeps a whole lot of global state in lieu of 
    /// passing parameters. I recommend comparing it against AngryAntPathway
    /// and OpenSteerUtility as an example of how to disentangle the state. I
	/// have barely modified it from the original class file.
	/// </remarks>
    public class PolylinePathway : Pathway
    {
        int pointCount;
        Vector3[] points;
        float radius;

        float segmentLength;
        float segmentProjection;
        Vector3 chosen;
        Vector3 segmentNormal;

        float[] lengths;
        Vector3[] normals;
        float totalPathLength;

        PolylinePathway () {
            
        }

        // construct a PolylinePathway given the number of points (vertices),
        // an array of points, and a path radius.
        PolylinePathway (Vector3[] _points, float _radius, bool _cyclic)
        {
            initialize (_points, _radius, _cyclic);
        }

        // utility for constructors in derived classes
        void initialize (Vector3[] _points, float _radius, bool _cyclic)
        {
            // set data members, allocate arrays
            radius = _radius;
            isCyclic = _cyclic;
            pointCount = _points.Length;
            totalPathLength = 0;
            if (isCyclic) 
            {
                pointCount++;
            }
            lengths = new float [pointCount];
            points  = new Vector3 [pointCount];
            normals = new Vector3 [pointCount];

            // loop over all points
            for (int i = 0; i < _points.Length; i++)
            {
                // copy in point locations, closing cycle when appropriate
                bool closeCycle = isCyclic && (i == pointCount-1);
                int j = closeCycle ? 0 : i;
                points[i] = _points[j];

                // for the end of each segment
                if (i > 0)
                {
                    // compute the segment length
                    normals[i] = points[i] - points[i-1];
                    lengths[i] = normals[i].magnitude;

                    // find the normalized vector parallel to the segment
                    normals[i] *= 1 / lengths[i];

                    // keep running total of segment lengths
                    totalPathLength += lengths[i];
                }
            }
        }

        // utility methods



        // assessor for total path length;
        protected override float GetTotalPathLength() 
        {
            return totalPathLength;
        }


        public override Vector3 mapPointToPath(Vector3 point, ref mapReturnStruct tStruct)
        {
            float d;
            float minDistance = float.MaxValue;// FLT_MAX;
            Vector3 onPath=Vector3.zero;

            // loop over all segments, find the one nearest to the given point
            for (int i = 1; i < pointCount; i++)
            {
                segmentLength = lengths[i];
                segmentNormal = normals[i];
                d = pointToSegmentDistance (point, points[i-1], points[i]);
                if (d < minDistance)
                {
                    minDistance = d;
                    onPath = chosen;
                    tStruct.tangent = segmentNormal;
                }
            }

            // measure how far original point is outside the Pathway's "tube"
            tStruct.outside = (onPath - point).magnitude - radius;

            // return point on path
            return onPath;
        }

        public override float mapPointToPathDistance(Vector3 point)
        {
            float d;
            float minDistance = float.MaxValue;
            float segmentLengthTotal = 0;
            float pathDistance = 0;

            for (int i = 1; i < pointCount; i++)
            {
                segmentLength = lengths[i];
                segmentNormal = normals[i];
                d = pointToSegmentDistance (point, points[i-1], points[i]);
                if (d < minDistance)
                {
                    minDistance = d;
                    pathDistance = segmentLengthTotal + segmentProjection;
                }
                segmentLengthTotal += segmentLength;
            }

            // return distance along path of onPath point
            return pathDistance;
        }

        public override Vector3 mapPathDistanceToPoint(float pathDistance)
        {
            // clip or wrap given path distance according to cyclic flag
            float remaining = pathDistance;
            if (isCyclic)
            {
                remaining = (float)System.Math.IEEERemainder(pathDistance, totalPathLength);
            }
            else
            {
                if (pathDistance < 0) return points[0];
                if (pathDistance >= totalPathLength) return points [pointCount-1];
            }

            // step through segments, subtracting off segment lengths until
            // locating the segment that contains the original pathDistance.
            // Interpolate along that segment to find 3d point value to return.
            Vector3 result=Vector3.zero;
            for (int i = 1; i < pointCount; i++)
            {
                segmentLength = lengths[i];
                if (segmentLength < remaining)
                {
                    remaining -= segmentLength;
                }
                else
                {
                    float ratio = remaining / segmentLength;
                    result = Vector3.Lerp(points[i - 1], points[i], ratio);
                    break;
                }
            }
            return result;
        }


        // ----------------------------------------------------------------------------
        // computes distance from a point to a line segment 
        //
        // cwr: (I considered moving this to the vector library, but its too
        // tangled up with the internal state of the PolylinePathway instance)
        // 
        // RJM: Look into OpenSteerUtility for a version  of this method without
        // the entangled local state
        // 
        float pointToSegmentDistance (Vector3 point, Vector3 ep0, Vector3 ep1)
        {
            // convert the test point to be "local" to ep0
            Vector3 local = point - ep0;

            // find the projection of "local" onto "segmentNormal"
            segmentProjection = Vector3.Dot(segmentNormal, local);

            // handle boundary cases: when projection is not on segment, the
            // nearest point is one of the endpoints of the segment
            if (segmentProjection < 0)
            {
                chosen = ep0;
                segmentProjection = 0;
                return (point- ep0).magnitude;
            }
            if (segmentProjection > segmentLength)
            {
                chosen = ep1;
                segmentProjection = segmentLength;
                return (point-ep1).magnitude;
            }

            // otherwise nearest point is projection point on segment
            chosen = segmentNormal * segmentProjection;
            chosen +=  ep0;
            return Vector3.Distance(point, chosen);
        }
    }
}
