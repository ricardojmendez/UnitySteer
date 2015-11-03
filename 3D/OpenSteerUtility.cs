using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace UnitySteer
{
    public static class OpenSteerUtility
    {
        /// <summary>
        /// Returns a random unit-length vector on the X/Z plane.
        /// </summary>
        /// <returns>The unit vector on XZ plane.</returns>
        public static Vector3 RandomUnitVectorOnXZPlane()
        {
            var tVector = Random.insideUnitSphere;
            tVector.y = 0;
            tVector.Normalize();
            return tVector;
        }

        public static Vector3 LimitMaxDeviationAngle(Vector3 source, float cosineOfConeAngle, Vector3 basis)
        {
            return VecLimitDeviationAngleUtility(true, // force source INSIDE cone
                source,
                cosineOfConeAngle,
                basis);
        }

        public static Vector3 VecLimitDeviationAngleUtility(bool insideOrOutside, Vector3 source,
            float cosineOfConeAngle, Vector3 basis)
        {
            // immediately return zero length input vectors
            var sourceLength = source.magnitude;
            if (sourceLength == 0) return source;

            // measure the angular deviation of "source" from "basis"
            // There doesn't seem to be a significant performance difference
            // between this and source.normalized, particularly since we 
            // needed the magnitude before anyway.
            var direction = source / sourceLength;
            var cosineOfSourceAngle = Vector3.Dot(direction, basis);

            // Simply return "source" if it already meets the angle criteria.
            // (note: we hope this top "if" gets compiled out since the flag
            // is a constant when the function is inlined into its caller)
            if (insideOrOutside)
            {
                // source vector is already inside the cone, just return it
                if (cosineOfSourceAngle >= cosineOfConeAngle) return source;
            }
            else
            {
                // source vector is already outside the cone, just return it
                if (cosineOfSourceAngle <= cosineOfConeAngle) return source;
            }

            // find the portion of "source" that is perpendicular to "basis"
            var perp = PerpendicularComponent(source, basis);

            // construct a new vector whose length equals the source vector,
            // and lies on the intersection of a plane (formed the source and
            // basis vectors) and a cone (whose axis is "basis" and whose
            // angle corresponds to cosineOfConeAngle)
            var perpDist = (float) Math.Sqrt(1 - (cosineOfConeAngle * cosineOfConeAngle));
            var c0 = basis * cosineOfConeAngle;
            var c1 = perp.normalized * perpDist;
            return (c0 + c1) * sourceLength;
        }

        /// <summary>
        /// Returns the parallel component to a vector.
        /// </summary>
        /// <returns>The parallel component.</returns>
        /// <param name="source">Source.</param>
        /// <param name="unitBasis">Unit basis vector.</param>
        public static Vector3 ParallelComponent(Vector3 source, Vector3 unitBasis)
        {
            var projection = Vector3.Dot(source, unitBasis);
            return unitBasis * projection;
        }


        /// <summary>
        /// Returns the component of vector perpendicular to a unit basis vector.
        /// </summary>
        /// <returns>The component.</returns>
        /// <param name="source">Source vector.</param>
        /// <param name="unitBasis">Basis. Should be a unit vector.</param>
        public static Vector3 PerpendicularComponent(Vector3 source, Vector3 unitBasis)
        {
            return source - ParallelComponent(source, unitBasis);
        }

        public static Vector3 SphericalWrapAround(Vector3 source, Vector3 center, float radius)
        {
            var offset = source - center;
            var r = offset.magnitude;

            var result = (r > radius) ? source + ((offset / r) * radius * -2) : source;
            return result;
        }

        /// <summary>
        /// Does a scalar random walk from an initial value, within boundaries.
        /// </summary>
        /// <returns>The next random walk value.</returns>
        /// <param name="initial">Value to work from.</param>
        /// <param name="walkSpeed">Walk speed.</param>
        /// <param name="min">Minimum for the next value.</param>
        /// <param name="max">Maximum for the next value.</param>
        public static float ScalarRandomWalk(float initial, float walkSpeed, float min, float max)
        {
            var next = initial + ((Random.value * 2 - 1) * walkSpeed);
            next = Mathf.Clamp(next, min, max);
            return next;
        }

        /// <summary>
        /// Compares x with an interfal and returns a comparison value.
        /// </summary>
        /// <returns>-1 if x is below the lower bound, +1 if above it, and 0 if it is in the described interval.</returns>
        /// <param name="x">Amount to compare.</param>
        /// <param name="lowerBound">Lower bound.</param>
        /// <param name="upperBound">Upper bound.</param>
        public static int IntervalComparison(float x, float lowerBound, float upperBound)
        {
            if (x < lowerBound) return -1;
            if (x > upperBound) return +1;
            return 0;
        }

        /// <summary>
        /// Computes distance from a point to a line segment 
        ///
        /// Whenever possible the segment's normal and length should be calculated 
        /// in advance for performance reasons, if we're dealing with a known point 
        /// sequence in a path, but we provide for the case where the values aren't
        /// sent.
        /// </summary>
        /// <returns>The distance from the point to the one chosen along the segment.</returns>
        /// <param name="point">Point to calculate the distance from</param>
        /// <param name="ep0">Segment start</param>
        /// <param name="ep1">Segment end</param>
        /// <param name="segmentProjection">Segment projection.</param>
        public static float PointToSegmentDistance(Vector3 point, Vector3 ep0, Vector3 ep1,
            ref float segmentProjection)
        {
            var cp = Vector3.zero;
            return PointToSegmentDistance(point, ep0, ep1, ref cp, ref segmentProjection);
        }

        /// <summary>
        /// Computes distance from a point to a line segment and fills in the
        /// information for the closest chosen point along the segment.
        ///
        /// Whenever possible the segment's normal and length should be calculated 
        /// in advance for performance reasons, if we're dealing with a known point 
        /// sequence in a path, but we provide for the case where the values aren't
        /// sent.
        /// </summary>
        /// <param name="point">Point to calculate the distance from</param>
        /// <param name="ep0">Segment start</param>
        /// <param name="ep1">Segment end</param>
        /// <param name="chosenPoint">Chosen closest point along the segment</param>
        /// <remarks>Not crazy about having the segment length as a separate
        /// parameter, since it could introduce bugs where the wrong length is
        /// passed, but it allows us to have the segments pre-calculated</remarks>
        /// <returns>The distance from the point to the one chosen along the segment.</returns>
        public static float PointToSegmentDistance(Vector3 point, Vector3 ep0, Vector3 ep1,
            ref Vector3 chosenPoint)
        {
            float sp = 0;
            return PointToSegmentDistance(point, ep0, ep1, ref chosenPoint, ref sp);
        }

        /// <summary>
        /// Computes distance from a point to a line segment and fills in the
        /// information for both the closest chosen point along the segment, and 
        /// the segment projection.
        ///
        /// Whenever possible the segment's normal and length should be calculated 
        /// in advance for performance reasons, if we're dealing with a known point 
        /// sequence in a path, but we provide for the case where the values aren't
        /// sent.
        /// </summary>
        /// <param name="point">Point to calculate the distance from</param>
        /// <param name="ep0">Segment start</param>
        /// <param name="ep1">Segment end</param>
        /// <param name="chosenPoint">Chosen closest point along the segment</param>
        /// <param name="segmentProjection">Segment projection.</param>
        /// <returns>The distance from the point to the one chosen along the segment.</returns>
        public static float PointToSegmentDistance(Vector3 point, Vector3 ep0, Vector3 ep1,
            ref Vector3 chosenPoint,
            ref float segmentProjection)
        {
            var normal = ep1 - ep0;
            var length = normal.magnitude;
            normal *= 1 / length;

            return PointToSegmentDistance(point, ep0, ep1, normal, length,
                ref chosenPoint, ref segmentProjection);
        }

        /// <summary>
        /// Computes distance from a point to a line segment and fills in the
        /// information for the segment projection for the closest point along
        /// the line segment.   
        ///
        /// Whenever possible the segment's normal and length should be calculated 
        /// in advance for performance reasons, if we're dealing with a known point 
        /// sequence in a path, but we provide for the case where the values aren't
        /// sent.
        /// </summary>
        /// <param name="point">Point to calculate the distance from</param>
        /// <param name="ep0">Segment start</param>
        /// <param name="ep1">Segment end</param>
        /// <param name="segmentNormal">Segment normal</param>
        /// <param name="segmentLength">Segment length</param>
        /// <param name="segmentProjection">Segment projection for the closest point</param>
        /// <remarks>Not crazy about having the segment length as a separate
        /// parameter, since it could introduce bugs where the wrong length is
        /// passed, but it allows us to have the segments pre-calculated</remarks>
        /// <returns>The distance from the point to the one chosen along the segment.</returns>
        public static float PointToSegmentDistance(Vector3 point, Vector3 ep0, Vector3 ep1,
            Vector3 segmentNormal, float segmentLength,
            ref float segmentProjection)
        {
            var cp = Vector3.zero;
            return PointToSegmentDistance(point, ep0, ep1, segmentNormal, segmentLength,
                ref cp, ref segmentProjection);
        }

        /// <summary>
        /// Computes distance from a point to a line segment and fills in the
        /// information for the closest chosen point along the segment.
        /// </summary>
        /// <param name="point">Point to calculate the distance from</param>
        /// <param name="ep0">Segment start</param>
        /// <param name="ep1">Segment end</param>
        /// <param name="segmentNormal">Segment normal</param>
        /// <param name="segmentLength">Segment length</param>
        /// <param name="chosenPoint">Chosen closest point along the segment</param>
        /// <remarks>Not crazy about having the segment length as a separate
        /// parameter, since it could introduce bugs where the wrong length is
        /// passed, but it allows us to have the segments pre-calculated</remarks>
        /// <returns>The distance from the point to the one chosen along the segment.</returns>
        public static float PointToSegmentDistance(Vector3 point, Vector3 ep0, Vector3 ep1,
            Vector3 segmentNormal, float segmentLength,
            ref Vector3 chosenPoint)
        {
            float sp = 0;
            return PointToSegmentDistance(point, ep0, ep1, segmentNormal, segmentLength,
                ref chosenPoint, ref sp);
        }


        /// <summary>
        /// Computes distance from a point to a line segment and fills in the
        /// information for both the closest chosen point along the segment, and 
        /// the segment projection.
        /// </summary>
        /// <param name="point">Point to calculate the distance from</param>
        /// <param name="ep0">Segment start</param>
        /// <param name="ep1">Segment end</param>
        /// <param name="segmentNormal">Segment normal</param>
        /// <param name="segmentLength">Segment length</param>
        /// <param name="chosenPoint">Chosen closest point along the segment</param>
        /// <param name="segmentProjection">Segment projection.</param>
        /// <remarks>Not crazy about having the segment length as a separate
        /// parameter, since it could introduce bugs where the wrong length is
        /// passed, but it allows us to have the segments pre-calculated</remarks>
        /// <returns>The distance from the point to the one chosen along the segment.</returns>
        public static float PointToSegmentDistance(Vector3 point, Vector3 ep0, Vector3 ep1,
            Vector3 segmentNormal, float segmentLength,
            ref Vector3 chosenPoint,
            ref float segmentProjection)
        {
            // convert the test point to be "local" to ep0
            var local = point - ep0;

            // find the projection of "local" onto "segmentNormal"
            segmentProjection = Vector3.Dot(segmentNormal, local);

            // handle boundary cases: when projection is not on segment, the
            // nearest point is one of the endpoints of the segment
            if (segmentProjection < 0)
            {
                chosenPoint = ep0;
                segmentProjection = 0;
                return (point - ep0).magnitude;
            }
            if (segmentProjection > segmentLength)
            {
                chosenPoint = ep1;
                segmentProjection = segmentLength;
                return (point - ep1).magnitude;
            }

            // otherwise nearest point is projection point on segment
            chosenPoint = segmentNormal * segmentProjection;
            chosenPoint += ep0;
            return Vector3.Distance(point, chosenPoint);
        }


        /// <summary>
        /// Returns the cosine for an angle in degrees
        /// </summary>
        /// <returns>Cosine.</returns>
        /// <param name="angle">Angle in degrees.</param>
        public static float CosFromDegrees(float angle)
        {
            return Mathf.Cos(angle * Mathf.Deg2Rad);
        }

        /// <summary>
        /// Returns an angle in degrees from a cosine
        /// </summary>
        /// <returns>Corresonding angle in degrees.</returns>
        /// <param name="cos">Cosine.</param>
        public static float DegreesFromCos(float cos)
        {
            return Mathf.Rad2Deg * Mathf.Acos(cos);
        }
    }
}