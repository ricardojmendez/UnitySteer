using System;
using System.Collections;
using System.Text;

namespace OpenSteerDotNet
{
    public class OpenSteerUtility
    {
        public static Vector3 interpolate(float alpha, Vector3 x0, Vector3 x1)
        {
            return x0 + ((x1 - x0) * alpha);
        }

        public static float interpolate(float alpha, float x0, float x1)
        {
            return x0 + ((x1 - x0) * alpha);
        }

        public static Vector3 RandomUnitVectorOnXZPlane ()
        {
            Vector3 tVector=RandomVectorInUnitRadiusSphere();
            tVector.y=0;
            tVector.Normalise();
            return tVector;
            //return RandomVectorInUnitRadiusSphere().setYtoZero().normalize();
        }

        public static Vector3 RandomVectorInUnitRadiusSphere ()
        {
            Vector3 v=Vector3.ZERO;

            do
            {
//                v=new Vector3((frandom01()*2) - 1,
//                       (frandom01()*2) - 1,
//                       (frandom01()*2) - 1);

                v = new Vector3((RandomGenerator.Singleton.nextFloat() * 2) - 1,
                        (RandomGenerator.Singleton.nextFloat() * 2) - 1,
                        (RandomGenerator.Singleton.nextFloat() * 2) - 1);
            }
            while (v.Length >= 1);

            return v;
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
            float sourceLength = source.Length;
            if (sourceLength == 0) return source;

            // measure the angular diviation of "source" from "basis"
            Vector3 direction = source / sourceLength;
            float cosineOfSourceAngle = direction.DotProduct (basis);

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

            // normalize that perpendicular
            
                
            Vector3 unitPerp = perp;//.normalize ();
            unitPerp.Normalise();

            // construct a new vector whose length equals the source vector,
            // and lies on the intersection of a plane (formed the source and
            // basis vectors) and a cone (whose axis is "basis" and whose
            // angle corresponds to cosineOfConeAngle)
            float perpDist = (float) System.Math.Sqrt (1 - (cosineOfConeAngle * cosineOfConeAngle));
            Vector3 c0 = basis * cosineOfConeAngle;
            Vector3 c1 = unitPerp * perpDist;
            return (c0 + c1) * sourceLength;
        }

        public static Vector3 parallelComponent (Vector3 source,Vector3 unitBasis)
        {
            float projection = source.DotProduct(unitBasis);
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
            return interpolate(clip(smoothRate, 0, 1),smoothedAccumulator,newValue);
        }

        public static float blendIntoAccumulator(float smoothRate, float newValue, float smoothedAccumulator)
        {
            return interpolate(clip(smoothRate, 0, 1), smoothedAccumulator, newValue);
        }

        public static float clip(float x, float min, float max)
        {
            if (x < min) return min;
            if (x > max) return max;
            return x;
        }

        public static Vector3 RandomUnitVector ()
        {
            Vector3 tVector = RandomVectorInUnitRadiusSphere();
            tVector.Normalise();
            return tVector;
        }

        public static Vector3 sphericalWrapAround (Vector3 source, Vector3 center, float radius)
        {
            Vector3 offset = source - center;
            float r = offset.Length;

            if (r > radius)
                return source + ((offset / r) * radius * -2);
            else
                return source;
        }
    }
}
