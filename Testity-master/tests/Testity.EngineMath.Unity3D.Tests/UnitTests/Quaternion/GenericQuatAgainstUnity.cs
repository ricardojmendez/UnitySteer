using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Testity.EngineMath.Unity3D.Tests
{
	[TestFixture]
	public static class GenericQuatAgainstUnity
	{

		//WARNING: We are unable to test some methods. This is an issue. The methods listed below we cannot test against Unity3D.
		//The reason for this is they make calls into C++ that we cannot make outside of Unity.
		/*
	private static extern Quaternion INTERNAL_CALL_AngleAxis(float angle, ref Vector3 axis);
	private static extern Quaternion INTERNAL_CALL_AxisAngle(ref Vector3 axis, float angle);
	private static extern Quaternion INTERNAL_CALL_FromToRotation(ref Vector3 fromDirection, ref Vector3 toDirection);
	private static extern Quaternion INTERNAL_CALL_Internal_FromEulerRad(ref Vector3 euler);
	private static extern void INTERNAL_CALL_Internal_ToAxisAngleRad(ref Quaternion q, out Vector3 axis, out float angle);
	private static extern Vector3 INTERNAL_CALL_Internal_ToEulerRad(ref Quaternion rotation);
	private static extern Quaternion INTERNAL_CALL_Inverse(ref Quaternion rotation);
	private static extern Quaternion INTERNAL_CALL_Lerp(ref Quaternion a, ref Quaternion b, float t);
	private static extern Quaternion INTERNAL_CALL_LerpUnclamped(ref Quaternion a, ref Quaternion b, float t);
	private static extern Quaternion INTERNAL_CALL_LookRotation(ref Vector3 forward, ref Vector3 upwards);
	private static extern Quaternion INTERNAL_CALL_Slerp(ref Quaternion a, ref Quaternion b, float t);
	private static extern Quaternion INTERNAL_CALL_SlerpUnclamped(ref Quaternion a, ref Quaternion b, float t);*/

		[Test(Author = "Andrew Blakely", Description = "Tests" + nameof(Quaternion<float>) + " initialization againstUnity3D.", TestOf = typeof(Quaternion<>))]
		[TestCase(0.0000235f, 5.654543f, 10.1234234f, 64)]
		[TestCase(5.3f, 6.5f, 7.6f, 8.09f)]
		[TestCase(1, 3, -4, 5.0f)]
		[TestCase(1, 3, -4, 5.000005340001f)]
		[TestCase(-1, 3, 4, 0)]
		[TestCase(1, 2, 3, 6.4f)]
		[TestCase(-0, 0, 0, -5)]
		[TestCase(-5.3f, 6.5f, 7.6f, 6.4f)]
		[TestCase(5.3f, 6.5f, 55, 56)]
		[TestCase(1f, 3f, -4f, 70)]
		[TestCase(1, 2, 3, 6.4f)]
		[TestCase(0, 0, 0, 0)]
		public static void Test_Quat_Generic_Initialization_Against_Unity(float a, float b, float c, float d)
		{
			//arrange
			Quaternion<float> genericQuat = new Quaternion<float>(a, b, c, d);
			UnityEngine.Quaternion unityQuat = new UnityEngine.Quaternion(a, b, c, d);

			//assert
			for (int i = 0; i < 4; i++)
				Assert.AreEqual(genericQuat[i], unityQuat[i]);
		}


		[Test(Author = "Andrew Blakely", Description = "Tests" + nameof(Quaternion<float>) + " multiplication with a vector againstUnity3D.", TestOf = typeof(Quaternion<>))]
		[TestCase(0.0000235f, 5.654543f, 10.1234234f, 64)]
		[TestCase(5.3f, 6.5f, 7.6f, 8.09f)]
		[TestCase(1, 3, -4, 5.0f)]
		[TestCase(1, 3, -4, 5.000005340001f)]
		[TestCase(-1, 3, 4, 0)]
		[TestCase(1, 2, 3, 6.4f)]
		[TestCase(-0, 0, 0, -5)]
		[TestCase(-5.3f, 6.5f, 7.6f, 6.4f)]
		[TestCase(5.3f, 6.5f, 55, 56)]
		[TestCase(1f, 3f, -4f, 70)]
		[TestCase(1, 2, 3, 6.4f)]
		[TestCase(0, 0, 0, 0)]
		[TestCase(6, 7, 32, .005f)]
		public static void Test_Quat_Generic_Vector_Multiplication_Against_Unity(float a, float b, float c, float d)
		{
			//arrange
			Quaternion<float> genericQuat = new Quaternion<float>(a, b, c, d);
			Vector3<float> genericVec3 = new Vector3<float>(b, c, d);

			//arrange
			UnityEngine.Quaternion unityQuat = new UnityEngine.Quaternion(a, b, c, d);
			UnityEngine.Vector3 unityVec3 = new UnityEngine.Vector3(b, c, d);

			//act
			Vector3<float> genericResultVec3 = genericQuat.RotateVector(genericVec3);
			UnityEngine.Vector3 unityResultVec3 = unityQuat * unityVec3;

			//assert
			for (int i = 0; i < 3; i++)
				//for Vector3 I rewrote the code so that it mostly matched UnityEngine after JIT values, not that I looked at the JIT compiled code but the values matched
				//after changing it. However, for some reason these do not line up like before. Operator overloads aren't equivalent to methods and something is happening
				//to make this not equivalent even though it's the same exact floating point math C# code.
				Assert.AreEqual(genericResultVec3[i], unityResultVec3[i], Math.Abs((((int)unityResultVec3[i]) * Vector3<float>.kEpsilon)));
		}

		[Test(Author = "Andrew Blakely", Description = "Tests" + nameof(Quaternion<float>) + " multiplication with a two quats Unity3D.", TestOf = typeof(Quaternion<>))]
		[TestCase(-5.3f, 6.5f, 7.6f, 0.0000235f, 5.654543f, 10.1234234f)]
		[TestCase(5.3f, 6.5f, 7.6f, 6.4f, 67.55f, 8.09f)]
		[TestCase(1, 3, -4, 5.0f, 2, -5.43333f)]
		[TestCase(1, 3, -4, 5.000005340001f, 2, -5.43333333f)]
		[TestCase(-1, 3, 4, 0, 0, 0)]
		[TestCase(1, 2, 3, 6.4f, 67.55f, 8.095454f)]
		[TestCase(-0, 0, 0, -5, -100.04332312f, -10.3333f)]
		[TestCase(-5.3f, 6.5f, 7.6f, 6.4f, 67.55f, 8.09f)]
		[TestCase(5.3f, 6.5f, -7.6f, 55, 56, 105.0522f)]
		[TestCase(1f, 3f, -4f, 7, 6, 0)]
		[TestCase(1, 2, 3, 6.4f, 67.55000054f, 8.0900001f)]
		[TestCase(0, 0, 0, 0, 0, 0)]
		public static void Test_Quat_Generic_QuatAndQuat_Multiplication_Against_Unity(float a, float b, float c, float d, float e, float f)
		{
			float g = (float)(new Random(System.DateTime.UtcNow.Millisecond).NextDouble());

			//arrange
			Quaternion<float> genericQuatOne = new Quaternion<float>(a, b, c, d);
			Quaternion<float> genericQuatTwo = new Quaternion<float>(d, e, f, g);

			UnityEngine.Quaternion unityQuatTwo = new UnityEngine.Quaternion(d, e, f, g);
			UnityEngine.Quaternion unityQuatOne = new UnityEngine.Quaternion(a, b, c, d);

			//act
			Quaternion<float> genericResult = genericQuatOne.MultiplyBy(genericQuatTwo);
			UnityEngine.Quaternion unityResult = unityQuatOne * unityQuatTwo;

			//assert
			for (int i = 0; i < 4; i++)//for some reason this works with precision. Maybe too much floating point math in the vector one.
				Assert.AreEqual(genericResult[i], unityResult[i]);
		}

		[Test(Author = "Andrew Blakely", Description = "Tests" + nameof(Quaternion<float>) + " multiplication with a two quats Unity3D.", TestOf = typeof(Quaternion<>))]
		[TestCase(0.0000235f, 5.654543f, 10.1234234f, 64)]
		[TestCase(5.3f, 6.5f, 7.6f, 8.09f)]
		[TestCase(1, 3, -4, 5.0f)]
		[TestCase(1, 3, -4, 5.000005340001f)]
		[TestCase(-1, 3, 4, 0)]
		[TestCase(1, 2, 3, 6.4f)]
		[TestCase(-0, 0, 0, -5)]
		[TestCase(-5.3f, 6.5f, 7.6f, 6.4f)]
		[TestCase(5.3f, 6.5f, 55, 56)]
		[TestCase(1f, 3f, -4f, 70)]
		[TestCase(1, 2, 3, 6.4f)]
		[TestCase(0, 0, 0, 0)]
		[TestCase(6, 7, 32, .005f)]
		public static void Test_Quat_Generic_Normalized_Against_System_Numerics_Quat(float a, float b, float c, float d)
		{
			//arrange
			Quaternion<float> genericQuat = new Quaternion<float>(a, b, c, d);
			//System.Numerics.Quaternion quat = new System.Numerics.Quaternion(a, b, c, d);

			//act
			Quaternion<float> genericNormalized = genericQuat.Normalize();

			//assert
			for (int i = 0; i < 4; i++)
				Assert.AreNotEqual(genericNormalized[i], float.NaN, "Index: {0} was NaN", i);

			//normalization is an unmanaged call in Unity3D. Can't test against it.
			Assert.AreEqual(Math.Sqrt(genericNormalized.Length()), genericNormalized.LengthSquared(), 1.1E-05, "Normalization or length failed for " + nameof(Quaternion<float>));
			Assert.AreEqual(genericNormalized.Length() == 0 ? 0.0f : 1.0f, Math.Sqrt(genericNormalized.Length()), 1.1E-05, "Normalization or length failed for " + nameof(Quaternion<float>));
		}
	}
}
