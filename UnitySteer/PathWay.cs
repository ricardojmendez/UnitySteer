// ----------------------------------------------------------------------------
//
// Ported to Unity by Ricardo J. Méndez http://www.arges-systems.com/
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
    public struct mapReturnStruct
    {
        public float outside;
        public Vector3 tangent;
    }

    public class Pathway
    {
        // Given an arbitrary point ("A"), returns the nearest point ("P") on
        // this path.  Also returns, via output arguments, the path tangent at
        // P and a measure of how far A is outside the Pathway's "tube".  Note
        // that a negative distance indicates A is inside the Pathway.
        public virtual Vector3 mapPointToPath(Vector3 point, mapReturnStruct tStruct) { return Vector3.zero; }
        //public virtual Vector3 mapPointToPath(Vector3 point, Vector3 tangent, float outside) { return Vector3.zero; }


        // given a distance along the path, convert it to a point on the path
        public virtual Vector3 mapPathDistanceToPoint(float pathDistance) { return Vector3.zero; }

        // Given an arbitrary point, convert it to a distance along the path.
        public virtual float mapPointToPathDistance(Vector3 point) { return 0; }

        // is the given point inside the path tube?
        public bool isInsidePath(Vector3 point)
        {

            //float outside;
            //Vector3 tangent;
            mapReturnStruct tStruct = new mapReturnStruct();

            mapPointToPath(point, tStruct);//tangent, outside);
            return tStruct.outside < 0;
        }

        // how far outside path tube is the given point?  (negative is inside)
        public float howFarOutsidePath(Vector3 point)
        {
            //float outside;
            //Vector3 tangent;
            mapReturnStruct tStruct = new mapReturnStruct();

            mapPointToPath(point, tStruct);//tangent, outside);
            return tStruct.outside;
        }
    }

    public class PolylinePathway : Pathway
    {
    
        int pointCount;
        Vector3[] points;
        float radius;
        bool cyclic;

        float segmentLength;
        float segmentProjection;
        Vector3 local;
        Vector3 chosen;
        Vector3 segmentNormal;

        float[] lengths;
        Vector3[] normals;
        float totalPathLength;

        PolylinePathway () {
            
        }

        // construct a PolylinePathway given the number of points (vertices),
        // an array of points, and a path radius.
        PolylinePathway (int _pointCount, Vector3[] _points, float _radius, bool _cyclic)
        {
            initialize (_pointCount, _points, _radius, _cyclic);
        }

        // utility for constructors in derived classes
        void initialize (int _pointCount, Vector3[] _points, float _radius, bool _cyclic)
        {
            // set data members, allocate arrays
            radius = _radius;
            cyclic = _cyclic;
            pointCount = _pointCount;
            totalPathLength = 0;
            if (cyclic) pointCount++;
            lengths = new float [pointCount];
            points  = new Vector3 [pointCount];
            normals = new Vector3 [pointCount];

            // loop over all points
            for (int i = 0; i < pointCount; i++)
            {
                // copy in point locations, closing cycle when appropriate
                bool closeCycle = cyclic && (i == pointCount-1);
                int j = closeCycle ? 0 : i;
                points[i] = _points[j];

                // for the end of each segment
                if (i > 0)
                {
                    // compute the segment length
                    normals[i] = points[i] - points[i-1];
                    lengths[i] = normals[i].magnitude;// ength();

                    // find the normalized vector parallel to the segment
                    normals[i] *= 1 / lengths[i];

                    // keep running total of segment lengths
                    totalPathLength += lengths[i];
                }
            }
        }

        // utility methods



        // assessor for total path length;
        float getTotalPathLength () {return totalPathLength;}


        public override Vector3 mapPointToPath(Vector3 point, mapReturnStruct tStruct)//Vector3 tangent, float outside)
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
            tStruct.outside = (onPath - point).magnitude - radius;//Vector3::distance (onPath, point) - radius;

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
            if (cyclic)
            {
                remaining = (float)System.Math.IEEERemainder(pathDistance, totalPathLength);
                //remaining = (float) fmod (pathDistance, totalPathLength);
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
        // (I considered moving this to the vector library, but its too
        // tangled up with the internal state of the PolylinePathway instance)


        float pointToSegmentDistance (Vector3 point, Vector3 ep0, Vector3 ep1)
        {
            // convert the test point to be "local" to ep0
            local = point - ep0;

            // find the projection of "local" onto "segmentNormal"
            segmentProjection = Vector3.Dot(segmentNormal, local);

            // handle boundary cases: when projection is not on segment, the
            // nearest point is one of the endpoints of the segment
            if (segmentProjection < 0)
            {
                chosen = ep0;
                segmentProjection = 0;
                return (point- ep0).magnitude;//Vector3::distance (point, ep0);
            }
            if (segmentProjection > segmentLength)
            {
                chosen = ep1;
                segmentProjection = segmentLength;
                return (point-ep1).magnitude;//Vector3::distance (point, ep1);
            }

            // otherwise nearest point is projection point on segment
            chosen = segmentNormal * segmentProjection;
            chosen +=  ep0;
            return Vector3.Distance(point, chosen);//::distance (point, chosen);
        }

        
    }


}
