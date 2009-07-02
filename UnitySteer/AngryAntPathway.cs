// ----------------------------------------------------------------------------
//
// Written by Ricardo J. Méndez http://www.arges-systems.com/ based on 
// OpenSteer's PolylinePathway.
// 
// This class is provided to ease the use of AngryAnt's Path. It should be
// removed if you do not intend to use Path, since otherwise it'll generate
// compilation errors.
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
using System.Collections;
using PathLibrary;
using UnityEngine;

namespace UnitySteer
{
    public class AngryAntPathway : Pathway
    {
        private ArrayList   path;
        private float       totalPathLength;
        
        private float[]     lengths;
        private Vector3[]   normals;
        private Vector3[]   points;
        
        // Minimum radius along the path. It could be gleaned from the
        // NetworkAssets.
        private float       radius; 
        
        float segmentLength;
        Vector3 segmentNormal;
        
        AngryAntPathway () {
            
        }

        // construct a PolylinePathway given the number of points (vertices),
        // an array of points, and a path radius.
        AngryAntPathway (ArrayList path, float radius, bool cyclic)
        {
            initialize(path, radius, cyclic);
        }
        
        protected override float GetTotalPathLength()
        {
            return totalPathLength;
        }
        
        void initialize (ArrayList path, float radius, bool cyclic)
        {
            this.path = path;
            this.radius  = radius;
            this.isCyclic = cyclic;
            
            // set data members, allocate arrays
            int pointCount = path.Count;
            totalPathLength = 0;
            if (cyclic) 
            {
                pointCount++;
            }
            points  = new Vector3[pointCount];
            lengths = new float[pointCount];
            normals = new Vector3[pointCount];

            // loop over all points
            for (int i = 0; i < path.Count; i++)
            {
                // copy in point locations, closing cycle when appropriate
                bool closeCycle = cyclic && (i == pointCount-1);
                int j = closeCycle ? 0 : i;
                points[i] = ((ConnectionAsset)path[j]).To.AbsolutePosition;

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
        

        public override Vector3 mapPointToPath(Vector3 point, ref mapReturnStruct tStruct)
        {
            float d;
            float minDistance = float.MaxValue;
            Vector3 onPath = Vector3.zero;

            // loop over all segments, find the one nearest to the given point
            for (int i = 1; i < points.Length; i++)
            {
                segmentLength = lengths[i];
                segmentNormal = normals[i];
                Vector3 chosenPoint = Vector3.zero;
                d = pointToSegmentDistance (point, points[i-1], points[i], ref chosenPoint);
                if (d < minDistance)
                {
                    minDistance = d;
                    onPath = chosenPoint;
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

            for (int i = 1; i < points.Length; i++)
            {
                float segmentProjection = 0;
                segmentLength = lengths[i];
                segmentNormal = normals[i];
                d = pointToSegmentDistance (point, points[i-1], points[i], ref segmentProjection);
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
            if (IsCyclic)
            {
                remaining = (float)System.Math.IEEERemainder(pathDistance, totalPathLength);
            }
            else
            {
                if (pathDistance < 0) return points[0];
                if (pathDistance >= totalPathLength) return points [points.Length-1];
            }

            // step through segments, subtracting off segment lengths until
            // locating the segment that contains the original pathDistance.
            // Interpolate along that segment to find 3d point value to return.
            Vector3 result=Vector3.zero;
            for (int i = 1; i < points.Length; i++)
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
        float pointToSegmentDistance (Vector3 point, Vector3 ep0, Vector3 ep1, ref float segmentProjection)
        {
            Vector3 cp = Vector3.zero;
            return pointToSegmentDistance(point, ep0, ep1, ref cp, ref segmentProjection);
        }

        float pointToSegmentDistance (Vector3 point, Vector3 ep0, Vector3 ep1, ref Vector3 chosenPoint)
        {
            float sp = 0;
            return pointToSegmentDistance(point, ep0, ep1, ref chosenPoint, ref sp);
        }

        float pointToSegmentDistance (Vector3 point, Vector3 ep0, Vector3 ep1, 
            ref Vector3 chosenPoint, ref float segmentProjection)
        {
            // convert the test point to be "local" to ep0
            Vector3 local = point - ep0;

            // find the projection of "local" onto "segmentNormal"
            segmentProjection = Vector3.Dot(segmentNormal, local);

            // handle boundary cases: when projection is not on segment, the
            // nearest point is one of the endpoints of the segment
            if (segmentProjection < 0)
            {
                chosenPoint = ep0;
                segmentProjection = 0;
                return (point- ep0).magnitude;
            }
            if (segmentProjection > segmentLength)
            {
                chosenPoint = ep1;
                segmentProjection = segmentLength;
                return (point-ep1).magnitude;
            }

            // otherwise nearest point is projection point on segment
            chosenPoint = segmentNormal * segmentProjection;
            chosenPoint +=  ep0;
            return Vector3.Distance(point, chosenPoint);
        }
    }
}