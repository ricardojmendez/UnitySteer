using System.Collections;
using System.Text;
using UnityEngine;

namespace UnitySteer
{
    public class OpenSteerUtility
    {
        public static Vector3 RandomUnitVectorOnXZPlane ()
        {
            Vector3 tVector = Random.insideUnitSphere;
            tVector.y=0;
            tVector.Normalize();
            return tVector;
            //return RandomVectorInUnitRadiusSphere().setYtoZero().normalize();
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

            // measure the angular diviation of "source" from "basis"
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
    }
}
