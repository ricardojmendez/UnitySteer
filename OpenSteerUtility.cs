using System.Collections;
using System.Text;
using UnityEngine;

namespace UnitySteer
{
    public static class OpenSteerUtility
    {
        public static Vector3 RandomUnitVectorOnXZPlane ()
        {
            Vector3 tVector = Random.insideUnitSphere;
            tVector.y=0;
            tVector.Normalize();
            return tVector;
        }

        public static Vector3 limitMaxDeviationAngle (Vector3 source, float cosineOfConeAngle, Vector3 basis)
        {
            return vecLimitDeviationAngleUtility (true, // force source INSIDE cone
                                                  source,
                                                  cosineOfConeAngle,
                                                  basis);
        }
         
        public static Vector3 vecLimitDeviationAngleUtility (bool insideOrOutside, Vector3 source, float cosineOfConeAngle, Vector3 basis)
        {
            // immediately return zero length input vectors
            float sourceLength = source.magnitude;
            if (sourceLength == 0) return source;

            // measure the angular deviation of "source" from "basis"
			// There doesn't seem to be a significant performance difference
			// between this and source.normalized, particularly since we 
			// needed the magnitude before anyway.
            Vector3 direction = source / sourceLength;
            float cosineOfSourceAngle = Vector3.Dot(direction, basis);

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
            Vector3 perp = perpendicularComponent(source,basis);

            // construct a new vector whose length equals the source vector,
            // and lies on the intersection of a plane (formed the source and
            // basis vectors) and a cone (whose axis is "basis" and whose
            // angle corresponds to cosineOfConeAngle)
            float perpDist = (float) System.Math.Sqrt (1 - (cosineOfConeAngle * cosineOfConeAngle));
            Vector3 c0 = basis * cosineOfConeAngle;
            Vector3 c1 = perp.normalized * perpDist;
            return (c0 + c1) * sourceLength;
        }

        public static Vector3 parallelComponent (Vector3 source,Vector3 unitBasis)
        {
            float projection = Vector3.Dot(source, unitBasis);
            return unitBasis * projection;
        }

        // return component of vector perpendicular to a unit basis vector
        // (IMPORTANT NOTE: assumes "basis" has unit magnitude (length==1))
        public static Vector3 perpendicularComponent (Vector3 source, Vector3 unitBasis)
        {
            return source - parallelComponent(source,unitBasis);
        }

        public static Vector3 blendIntoAccumulator(float smoothRate, Vector3 newValue, Vector3 smoothedAccumulator)
        {
            return Vector3.Lerp(smoothedAccumulator, newValue, Mathf.Clamp(smoothRate, 0, 1));
        }

        public static float blendIntoAccumulator(float smoothRate, float newValue, float smoothedAccumulator)
        {
            return Mathf.Lerp(smoothedAccumulator, newValue, Mathf.Clamp(smoothRate, 0, 1));
        }

        public static Vector3 sphericalWrapAround (Vector3 source, Vector3 center, float radius)
        {
            Vector3 offset = source - center;
            float r = offset.magnitude;

            if (r > radius)
                return source + ((offset / r) * radius * -2);
            else
                return source;
        }
		

		public static float scalarRandomWalk (float initial, float walkspeed, float min, float max)
		{
			float next = initial + ((UnityEngine.Random.value * 2 - 1) * walkspeed);
			next = Mathf.Clamp(next, min, max);
			return next;
		}		
		
		public static int intervalComparison (float x, float lowerBound, float upperBound)
		{
			if (x < lowerBound) return -1;
			if (x > upperBound) return +1;
			return 0;
		}		
        
        // ----------------------------------------------------------------------------
        // Computes distance from a point to a line segment 
        //
        // Whenever possible the segment's normal and length should be calculated 
        // in advance for performance reasons, if we're dealing with a known point 
        // sequence in a path, but we provide for the case where the values aren't
        // sent.
        //
        public static float PointToSegmentDistance(Vector3 point, Vector3 ep0, Vector3 ep1, 
                                                   ref float segmentProjection)
        {
            Vector3 cp = Vector3.zero;
            return PointToSegmentDistance(point, ep0, ep1, ref cp, ref segmentProjection);
        }

        public static float PointToSegmentDistance(Vector3 point, Vector3 ep0, Vector3 ep1, 
                                                   ref Vector3 chosenPoint)
        {
            float sp = 0;
            return PointToSegmentDistance(point, ep0, ep1, ref chosenPoint, ref sp);
        }
        
        public static float PointToSegmentDistance(Vector3 point, Vector3 ep0, Vector3 ep1,
                                                   ref Vector3 chosenPoint, 
                                                   ref float segmentProjection)
        {
            Vector3 normal = ep1-ep0;
            float   length = normal.magnitude;
            normal *= 1/length;
            
            return PointToSegmentDistance(point, ep0, ep1, normal, length, 
                                          ref chosenPoint, ref segmentProjection);
        }        
        
        public static float PointToSegmentDistance(Vector3 point, Vector3 ep0, Vector3 ep1, 
                                                   Vector3 segmentNormal, float segmentLength,
                                                   ref float segmentProjection)
        {
            Vector3 cp = Vector3.zero;
            return PointToSegmentDistance(point, ep0, ep1, segmentNormal, segmentLength, 
                                          ref cp, ref segmentProjection);
        }

        public static float PointToSegmentDistance(Vector3 point, Vector3 ep0, Vector3 ep1, 
                                                   Vector3 segmentNormal, float segmentLength,
                                                   ref Vector3 chosenPoint)
        {
            float sp = 0;
            return PointToSegmentDistance(point, ep0, ep1, segmentNormal, segmentLength, 
                                          ref chosenPoint, ref sp);
        }
        

        public static float PointToSegmentDistance(Vector3 point, Vector3 ep0, Vector3 ep1, 
                                                   Vector3 segmentNormal, float segmentLength,
                                                   ref Vector3 chosenPoint, 
                                                   ref float segmentProjection)
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
        

        public static float CosFromDegrees(float angle)
        {
            return Mathf.Cos(angle * Mathf.Deg2Rad);
        }
        
        public static float DegreesFromCos(float cos)
        {
            return Mathf.Rad2Deg * Mathf.Acos(cos);
        }
        
        
    }
}
