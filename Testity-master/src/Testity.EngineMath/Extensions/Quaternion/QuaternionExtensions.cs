using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Testity.EngineMath
{
	public static class QuaternionExtensions
	{
		const float radToDegFloat = (float)(180.0 / Math.PI);
		const float degToRadFloat = (float)(Math.PI / 180.0);

		const float radToDegDouble = (float)(180.0 / Math.PI);
		const float degToRadDouble = (float)(Math.PI / 180.0);

		#region Ref Quat Operations

		#region Ref Dot
		/// <summary>
		///   <para>The dot product between two rotations.</para>
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		internal static float DotRef(ref Quaternion<float> a, ref Quaternion<float> b)
		{
			return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
		}

		/// <summary>
		///   <para>The dot product between two rotations.</para>
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		internal static double DotRef(ref Quaternion<double> a, ref Quaternion<double> b)
		{
			return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
		}
		#endregion

		#endregion

		#region ToEulerAngles

		#region Float Implementation
		public static Vector3<float> EulerAngles(this Quaternion<float> quat)
		{
			//http://stackoverflow.com/a/12122899/4184238
			float sqw = quat.w * quat.w;
			float sqx = quat.x * quat.x;
			float sqy = quat.y * quat.y;
			float sqz = quat.z * quat.z;
			float unit = sqx + sqy + sqz + sqw; // if normalised is one, otherwise is correction factor
			float test = quat.x * quat.w - quat.y * quat.z;
			Vector3<float> v = new Vector3<float>(0, 0, 0);

			if (test > 0.4995f * unit)
			{ // singularity at north pole
				v.y = (float)(2f * Math.Atan2(quat.y, quat.x));
				v.x = (float)(Math.PI / 2);
				v.z = 0;
				return NormalizeAngles(v * radToDegFloat);
			}
			if (test < -0.4995f * unit)
			{ // singularity at south pole
				v.y = (float)(-2f * Math.Atan2(quat.y, quat.x));
				v.x = (float)(-Math.PI / 2);
				v.z = 0;
				return NormalizeAngles(v * radToDegFloat);
			}

			Quaternion<float> tempQuat = new Quaternion<float>(quat.w, quat.z, quat.x, quat.y);

			//This version of the math can be found here: http://www.gamedev.net/topic/597324-quaternion-to-euler-angles-and-back-why-is-the-rotation-changing/
			/*eX = atan2(-2*(qy*qz-qw*qx), qw*qw-qx*qx-qy*qy+qz*qz);
			eY = asin(2*(qx*qz + qw*qy));
			eZ = atan2(-2*(qx*qy-qw*qz), qw*qw+qx*qx-qy*qy-qz*qz);*/
			//v.x = (float)Math.Atan2(-2f * (tempQuat.y * tempQuat.z - tempQuat.w * tempQuat.x), tempQuat.w * tempQuat.w - tempQuat.x * tempQuat.x - tempQuat.y * tempQuat.y + tempQuat.z * tempQuat.z);
			//v.y = (float)Math.Asin(2f * (tempQuat.x * tempQuat.z + tempQuat.w * tempQuat.y));
			//v.z = (float)Math.Atan2(-2 * (tempQuat.x * tempQuat.y - tempQuat.w * tempQuat.z), tempQuat.w * tempQuat.w + tempQuat.x * tempQuat.x - tempQuat.y * tempQuat.y - tempQuat.z * tempQuat.z);

			//These provide the best representation of euler angles compared to Unity. In fact, they're higher precision.
			v.y = (float)Math.Atan2(2f * tempQuat.x * tempQuat.w + 2f * tempQuat.y * tempQuat.z, 1 - 2f * (tempQuat.z * tempQuat.z + tempQuat.w * tempQuat.w));     // Yaw
			v.x = (float)Math.Asin(2f * (tempQuat.x * tempQuat.z - tempQuat.w * tempQuat.y));                             // Pitch
			v.z = (float)Math.Atan2(2f * tempQuat.x * tempQuat.y + 2f * tempQuat.z * tempQuat.w, 1 - 2f * (tempQuat.y * tempQuat.y + tempQuat.z * tempQuat.z));      // Roll

			return NormalizeAngles(v * radToDegFloat);
		}

		private static Vector3<float> NormalizeAngles(Vector3<float> angles)
		{
			//http://stackoverflow.com/a/12122899/4184238
			angles.x = NormalizeAngle(angles.x);
			angles.y = NormalizeAngle(angles.y);
			angles.z = NormalizeAngle(angles.z);
			return angles;
		}

		static float NormalizeAngle(float angle)
		{
			//This is a correction after testing in Unity. Unity always has postive euler angles.
			//This will make it postive.
			float modAngle = angle % 360.0f;
			if (modAngle < 0.0f)
				return modAngle + 360.0f;
			else
				return modAngle;

			//return angle % 360.0f;
		}
		#endregion

		#region Double Implementation
		public static Vector3<double> EulerAngles(this Quaternion<double> quat)
		{
			double rad2Deg = 57.29578f;

			//http://stackoverflow.com/a/12122899/4184238
			double sqw = quat.w * quat.w;
			double sqx = quat.x * quat.x;
			double sqy = quat.y * quat.y;
			double sqz = quat.z * quat.z;
			double unit = sqx + sqy + sqz + sqw; // if normalised is one, otherwise is correction factor
			double test = quat.x * quat.w - quat.y * quat.z;
			Vector3<double> v;

			if (test > 0.4995f * unit)
			{ // singularity at north pole
				v.y = 2d * Math.Atan2(quat.y, quat.x);
				v.x = Math.PI / 2d;
				v.z = 0d;
				return NormalizeAngles(v * rad2Deg);
			}
			if (test < -0.4995f * unit)
			{ // singularity at south pole
				v.y = -2d * Math.Atan2(quat.y, quat.x);
				v.x = -Math.PI / 2;
				v.z = 0;
				return NormalizeAngles(v * rad2Deg);
			}
			Quaternion<double> tempQuat = new Quaternion<double>(quat.w, quat.z, quat.x, quat.y);

			//These provide the best representation of euler angles compared to Unity. In fact, they're higher precision.
			v.y = Math.Atan2(2f * tempQuat.x * tempQuat.w + 2f * tempQuat.y * tempQuat.z, 1 - 2f * (tempQuat.z * tempQuat.z + tempQuat.w * tempQuat.w));     // Yaw
			v.x = Math.Asin(2f * (tempQuat.x * tempQuat.z - tempQuat.w * tempQuat.y));                             // Pitch
			v.z = Math.Atan2(2f * tempQuat.x * tempQuat.y + 2f * tempQuat.z * tempQuat.w, 1 - 2f * (tempQuat.y * tempQuat.y + tempQuat.z * tempQuat.z));      // Roll
			return NormalizeAngles(v * rad2Deg);
		}

		private static Vector3<double> NormalizeAngles(Vector3<double> angles)
		{
			//http://stackoverflow.com/a/12122899/4184238
			angles.x = NormalizeAngle(angles.x);
			angles.y = NormalizeAngle(angles.y);
			angles.z = NormalizeAngle(angles.z);
			return angles;
		}

		static double NormalizeAngle(double angle)
		{
			//This is a correction after testing in Unity. Unity always has postive euler angles.
			//This will make it postive.
			double modAngle = angle % 360.0d;
			if (modAngle < 0.0d)
				return modAngle + 360.0d;
			else
				return modAngle;

			//return angle % 360.0f;
		}
		#endregion

		#endregion

		#region FromEulerAngles

		#region Float Implementation
		/// <summary>
		///   <para>Returns a rotation that rotates z degrees around the z axis, x degrees around the x axis, and y degrees around the y axis (in that order).</para>
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		public static Quaternion<float> Euler(this Vector3<float> eulerVector)
		{
			Vector3<float> radianEulerVector = (eulerVector * (float)(Math.PI / 180.0));
			return Internal_FromEulerRad(ref radianEulerVector);
		}

		//from: https://gist.github.com/HelloKitty/91b7af87aac6796c3da9
		//and http://stackoverflow.com/questions/11492299/quaternion-to-euler-angles-algorithm-how-to-convert-to-y-up-and-between-ha
		private static Quaternion<float> Internal_FromEulerRad(ref Vector3<float> euler)
		{
			var yaw = euler.x;
			var pitch = euler.y;
			var roll = euler.z;
			float rollOver2 = roll * 0.5f;
			float sinRollOver2 = (float)Math.Sin((double)rollOver2);
			float cosRollOver2 = (float)Math.Cos((double)rollOver2);
			float pitchOver2 = pitch * 0.5f;
			float sinPitchOver2 = (float)Math.Sin((double)pitchOver2);
			float cosPitchOver2 = (float)Math.Cos((double)pitchOver2);
			float yawOver2 = yaw * 0.5f;
			float sinYawOver2 = (float)Math.Sin((double)yawOver2);
			float cosYawOver2 = (float)Math.Cos((double)yawOver2);
			Quaternion<float> result;
			result.x = cosYawOver2 * cosPitchOver2 * cosRollOver2 + sinYawOver2 * sinPitchOver2 * sinRollOver2;
			result.y = cosYawOver2 * cosPitchOver2 * sinRollOver2 - sinYawOver2 * sinPitchOver2 * cosRollOver2;
			result.z = cosYawOver2 * sinPitchOver2 * cosRollOver2 + sinYawOver2 * cosPitchOver2 * sinRollOver2;
			result.w = sinYawOver2 * cosPitchOver2 * cosRollOver2 - cosYawOver2 * sinPitchOver2 * sinRollOver2;
			return result;

		}
		#endregion

		#region Double Implementation
		/// <summary>
		///   <para>Returns a rotation that rotates z degrees around the z axis, x degrees around the x axis, and y degrees around the y axis (in that order).</para>
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		public static Quaternion<double> Euler(this Vector3<double> eulerVector)
		{
			Vector3<double> radianEulerVector = (eulerVector * (Math.PI / 180.0));
			return Internal_FromEulerRad(ref radianEulerVector);
		}

		//from: https://gist.github.com/HelloKitty/91b7af87aac6796c3da9
		//and http://stackoverflow.com/questions/11492299/quaternion-to-euler-angles-algorithm-how-to-convert-to-y-up-and-between-ha
		private static Quaternion<double> Internal_FromEulerRad(ref Vector3<double> euler)
		{
			var yaw = euler.x;
			var pitch = euler.y;
			var roll = euler.z;
			double rollOver2 = roll * 0.5d;
			double sinRollOver2 = Math.Sin((double)rollOver2);
			double cosRollOver2 = Math.Cos((double)rollOver2);
			double pitchOver2 = pitch * 0.5d;
			double sinPitchOver2 = Math.Sin((double)pitchOver2);
			double cosPitchOver2 = Math.Cos((double)pitchOver2);
			double yawOver2 = yaw * 0.5d;
			double sinYawOver2 = Math.Sin((double)yawOver2);
			double cosYawOver2 = Math.Cos((double)yawOver2);
			Quaternion<double> result;
			result.x = cosYawOver2 * cosPitchOver2 * cosRollOver2 + sinYawOver2 * sinPitchOver2 * sinRollOver2;
			result.y = cosYawOver2 * cosPitchOver2 * sinRollOver2 - sinYawOver2 * sinPitchOver2 * cosRollOver2;
			result.z = cosYawOver2 * sinPitchOver2 * cosRollOver2 + sinYawOver2 * cosPitchOver2 * sinRollOver2;
			result.w = sinYawOver2 * cosPitchOver2 * cosRollOver2 - cosYawOver2 * sinPitchOver2 * sinRollOver2;
			return result;

		}
		#endregion

		#endregion

		#region AngleBetween
		/// <summary>
		///   <para>Returns the angle in degrees between two rotations a and b.</para>
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		public static float AngleBetween(this Quaternion<float> a, Quaternion<float> b)
		{
			float single = a.Dot(b);
			return (float)Math.Acos(Math.Min(Math.Abs(single), 1f)) * 2f * 57.29578f;
		}

		/// <summary>
		///   <para>Returns the angle in degrees between two rotations a and b.</para>
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		public static double AngleBetween(this Quaternion<double> a, Quaternion<double> b)
		{
			double single = a.Dot(b);
			return (double)Math.Acos(Math.Min(Math.Abs(single), 1d)) * 2d * 57.29578d;
		}
		#endregion

		#region DotProduct
		/// <summary>
		///   <para>The dot product between two rotations.</para>
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		public static float Dot(this Quaternion<float> a, Quaternion<float> b)
		{
			return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
		}

		/// <summary>
		///   <para>The dot product between two rotations.</para>
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		public static double Dot(this Quaternion<double> a, Quaternion<double> b)
		{
			return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
		}
		#endregion

		#region RotateTowards
		/// <summary>
		///   <para>Rotates a rotation from towards to.</para>
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <param name="maxDegreesDelta"></param>
		public static Quaternion<float> RotateTowards(this Quaternion<float> from, Quaternion<float> to, float maxDegreesDelta)
		{
			float single = AngleBetween(from, to);

			if (single == 0.0f)
			{
				return to;
			}

			float single1 = Math.Min(1f, maxDegreesDelta / single);
			return SlerpUnclamped(from, to, single1);
		}

		/// <summary>
		///   <para>Rotates a rotation from towards to.</para>
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <param name="maxDegreesDelta"></param>
		public static Quaternion<double> RotateTowards(this Quaternion<double> from, Quaternion<double> to, double maxDegreesDelta)
		{
			double single = AngleBetween(from, to);

			if (single == 0.0f)
			{
				return to;
			}

			double single1 = Math.Min(1f, maxDegreesDelta / single);
			return SlerpUnclamped(from, to, single1);
		}
		#endregion

		#region Slerp Methods (Slerp/SlerpUnclamped)

		#region Slerp Unclamped

		#region Float Implementation
		public static Quaternion<float> SlerpUnclamped(this Quaternion<float> a, Quaternion<float> b, float t)
		{
			return INTERNAL_CALL_SlerpUnclamped(ref a, ref b, t);
		}

		private static Quaternion<float> INTERNAL_CALL_SlerpUnclamped(ref Quaternion<float> a, ref Quaternion<float> b, float t)
		{
			// if either input is zero, return the other.
			if (a.LengthSquared() == 0.0f)
			{
				if (b.LengthSquared() == 0.0f)
				{
					return Quaternion<float>.identity;
				}
				return b;
			}
			else if (b.LengthSquared() == 0.0f)
			{
				return a;
			}


			float cosHalfAngle = a.w * b.w + a.xyz.Dot(b.xyz);

			if (cosHalfAngle >= 1.0f || cosHalfAngle <= -1.0f)
			{
				// angle = 0.0f, so just return one input.
				return a;
			}
			else if (cosHalfAngle < 0.0f)
			{
				b.xyz = -b.xyz;
				b.w = -b.w;
				cosHalfAngle = -cosHalfAngle;
			}

			float blendA;
			float blendB;
			if (cosHalfAngle < 0.99f)
			{
				// do proper slerp for big angles
				float halfAngle = (float)System.Math.Acos(cosHalfAngle);
				float sinHalfAngle = (float)System.Math.Sin(halfAngle);
				float oneOverSinHalfAngle = 1.0f / sinHalfAngle;
				blendA = (float)System.Math.Sin(halfAngle * (1.0f - t)) * oneOverSinHalfAngle;
				blendB = (float)System.Math.Sin(halfAngle * t) * oneOverSinHalfAngle;
			}
			else
			{
				// do lerp if angle is really small.
				blendA = 1.0f - t;
				blendB = t;
			}

			Quaternion<float> result = new Quaternion<float>(blendA * a.xyz + blendB * b.xyz, blendA * a.w + blendB * b.w);
			if (result.LengthSquared() > 0.0f)
				return Normalize(result);
			else
				return Quaternion<float>.identity;
		}
		#endregion

		#region Double Implementation
		public static Quaternion<double> SlerpUnclamped(this Quaternion<double> a, Quaternion<double> b, double t)
		{
			return INTERNAL_CALL_SlerpUnclamped(ref a, ref b, t);
		}

		private static Quaternion<double> INTERNAL_CALL_SlerpUnclamped(ref Quaternion<double> a, ref Quaternion<double> b, double t)
		{
			// if either input is zero, return the other.
			if (a.LengthSquared() == 0.0f)
			{
				if (b.LengthSquared() == 0.0f)
				{
					return Quaternion<double>.identity;
				}
				return b;
			}
			else if (b.LengthSquared() == 0.0f)
			{
				return a;
			}


			double cosHalfAngle = a.w * b.w + a.xyz.Dot(b.xyz);

			if (cosHalfAngle >= 1.0f || cosHalfAngle <= -1.0f)
			{
				// angle = 0.0f, so just return one input.
				return a;
			}
			else if (cosHalfAngle < 0.0f)
			{
				b.xyz = -b.xyz;
				b.w = -b.w;
				cosHalfAngle = -cosHalfAngle;
			}

			double blendA;
			double blendB;
			if (cosHalfAngle < 0.99f)
			{
				// do proper slerp for big angles
				double halfAngle = (double)System.Math.Acos(cosHalfAngle);
				double sinHalfAngle = (double)System.Math.Sin(halfAngle);
				double oneOverSinHalfAngle = 1.0f / sinHalfAngle;
				blendA = (double)System.Math.Sin(halfAngle * (1.0f - t)) * oneOverSinHalfAngle;
				blendB = (double)System.Math.Sin(halfAngle * t) * oneOverSinHalfAngle;
			}
			else
			{
				// do lerp if angle is really small.
				blendA = 1.0f - t;
				blendB = t;
			}

			Quaternion<double> result = new Quaternion<double>(blendA * a.xyz + blendB * b.xyz, blendA * a.w + blendB * b.w);
			if (result.LengthSquared() > 0.0f)
				return Normalize(result);
			else
				return Quaternion<double>.identity;
		}
		#endregion

		#endregion

		#region Slerp

		#region Float Implementation
		/// <summary>
		///   <para>Spherically interpolates between /a/ and /b/ by t. The parameter /t/ is clamped to the range [0, 1].</para>
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <param name="t"></param>
		public static Quaternion<float> Slerp(this Quaternion<float> a, Quaternion<float> b, float t)
		{
			return INTERNAL_CALL_Slerp(ref a, ref b, t);
		}

		private static Quaternion<float> INTERNAL_CALL_Slerp(ref Quaternion<float> a, ref Quaternion<float> b, float t)
		{
			if (t > 1) t = 1;
			if (t < 0) t = 0;

			return INTERNAL_CALL_SlerpUnclamped(ref a, ref b, t);
		}
		#endregion

		#region Double Implementation
		/// <summary>
		///   <para>Spherically interpolates between /a/ and /b/ by t. The parameter /t/ is clamped to the range [0, 1].</para>
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <param name="t"></param>
		public static Quaternion<double> Slerp(this Quaternion<double> a, Quaternion<double> b, double t)
		{
			return INTERNAL_CALL_Slerp(ref a, ref b, t);
		}

		private static Quaternion<double> INTERNAL_CALL_Slerp(ref Quaternion<double> a, ref Quaternion<double> b, double t)
		{
			if (t > 1) t = 1;
			if (t < 0) t = 0;

			return INTERNAL_CALL_SlerpUnclamped(ref a, ref b, t);
		}
		#endregion

		#endregion

		#endregion

		//TODO: Write a lerp for quats. Currently it is unimplemented in: https://gist.github.com/HelloKitty/91b7af87aac6796c3da9
		#region Lerp methods (Lerp/LerpUnclamped)
		#endregion

		#region Length Methods
		/// <summary>
		/// Gets the length (magnitude) of the quaternion.
		/// </summary>
		/// <seealso cref="LengthSquared"/>
		public static float Length(this Quaternion<float> quat)
		{
			return (float)Math.Sqrt(quat.x * quat.x + quat.y * quat.y + quat.z * quat.z + quat.w * quat.w);
		}

		/// <summary>
		/// Gets the length (magnitude) of the quaternion.
		/// </summary>
		/// <seealso cref="LengthSquared"/>
		public static double Length(this Quaternion<double> quat)
		{
			return Math.Sqrt(quat.x * quat.x + quat.y * quat.y + quat.z * quat.z + quat.w * quat.w);
		}

		/// <summary>
		/// Gets the square of the quaternion length (magnitude).
		/// </summary>
		public static float LengthSquared(this Quaternion<float> quat)
		{
			return quat.x * quat.x + quat.y * quat.y + quat.z * quat.z + quat.w * quat.w;
		}

		/// <summary>
		/// Gets the square of the quaternion length (magnitude).
		/// </summary>
		public static double LengthSquared(this Quaternion<double> quat)
		{
			return quat.x * quat.x + quat.y * quat.y + quat.z * quat.z + quat.w * quat.w;
		}
		#endregion

		#region Normalization
		/// <summary>
		/// Scales the Quaternion to unit length.
		/// </summary>
		public static Quaternion<float> Normalize(this Quaternion<float> quat)
		{
			float length = quat.Length();

			//If the length is 0 then it's already normalized technically.
			if (length == 0.0f)
				return new Quaternion<float>(0, 0, 0, 0);

	float scale = 1.0f / length;
			quat.xyz *= scale;
			quat.w *= scale;

			return quat;
		}

		/// <summary>
		/// Scales the Quaternion to unit length.
		/// </summary>
		public static Quaternion<double> Normalize(this Quaternion<double> quat)
		{
			double length = quat.Length();

			//If the length is 0 then it's already normalized technically.
			if (length == 0.0d)
				return new Quaternion<double>(0d, 0d, 0d, 0d);

			double scale = 1.0f / length;

			quat.xyz *= scale;
			quat.w *= scale;

			return quat;
		}
		#endregion

		#region Inverse Methods
		/// <summary>
		///   <para>Returns the Inverse <see cref="Quaternion{TMathType}"/> of the rotation matrix.</para>
		/// </summary>
		/// <param name="rotation"></param>
		public static Quaternion<float> Inverse(this Quaternion<float> rotation)
		{
			float lengthSq = rotation.LengthSquared();

			if (lengthSq != 0.0)
			{
				float i = 1.0f / lengthSq;
				return new Quaternion<float>(rotation.xyz * -i, rotation.w * i);
			}

			return rotation;
		}

		/// <summary>
		///   <para>Returns the Inverse <see cref="Quaternion{TMathType}"/> of the rotation matrix.</para>
		/// </summary>
		/// <param name="rotation"></param>
		public static Quaternion<double> Inverse(this Quaternion<double> rotation)
		{
			double lengthSq = rotation.LengthSquared();

			if (lengthSq != 0.0)
			{
				double i = 1.0f / lengthSq;
				return new Quaternion<double>(rotation.xyz * -i, rotation.w * i);
			}

			return rotation;
		}
		#endregion

		#region LookRotation

		#region Float Implementation
		/// <summary>
		///   <para>Creates a rotation with the specified forward and upwards directions.</para>
		/// </summary>
		/// <param name="forward">The direction to look in.</param>
		/// <param name="upwards">The vector that defines in which direction up is.</param>
		public static Quaternion<float> LookRotation(this Vector3<float> forward, [DefaultValue("Vector3<float>.up")] Vector3<float> upwards)
		{
			return INTERNAL_CALL_LookRotation(ref forward, ref upwards);
		}

		/// <summary>
		///   <para>Creates a rotation with the specified forward and upwards directions.</para>
		/// </summary>
		/// <param name="forward">The direction to look in.</param>
		/// <param name="upwards">The vector that defines in which direction up is.</param>
		public static Quaternion<float> LookRotation(this Vector3<float> forward)
		{
			Vector3<float> up = Vector3<float>.up;
			return INTERNAL_CALL_LookRotation(ref forward, ref up);
		}

		//Form: https://gist.github.com/HelloKitty/91b7af87aac6796c3da9
		//and http://answers.unity3d.com/questions/467614/what-is-the-source-code-of-quaternionlookrotation.html
		private static Quaternion<float> INTERNAL_CALL_LookRotation(ref Vector3<float> forward, ref Vector3<float> up)
		{

			forward = forward.Normalized();
			Vector3<float> right = up.Cross(forward).Normalized();
			up = forward.Cross(right);
			var m00 = right.x;
			var m01 = right.y;
			var m02 = right.z;
			var m10 = up.x;
			var m11 = up.y;
			var m12 = up.z;
			var m20 = forward.x;
			var m21 = forward.y;
			var m22 = forward.z;


			float num8 = (m00 + m11) + m22;
			var quaternion = new Quaternion<float>();
			if (num8 > 0f)
			{
				var num = (float)Math.Sqrt(num8 + 1f);
				quaternion.w = num * 0.5f;
				num = 0.5f / num;
				quaternion.x = (m12 - m21) * num;
				quaternion.y = (m20 - m02) * num;
				quaternion.z = (m01 - m10) * num;
				return quaternion;
			}
			if ((m00 >= m11) && (m00 >= m22))
			{
				var num7 = (float)Math.Sqrt(((1f + m00) - m11) - m22);
				var num4 = 0.5f / num7;
				quaternion.x = 0.5f * num7;
				quaternion.y = (m01 + m10) * num4;
				quaternion.z = (m02 + m20) * num4;
				quaternion.w = (m12 - m21) * num4;
				return quaternion;
			}
			if (m11 > m22)
			{
				var num6 = (float)Math.Sqrt(((1f + m11) - m00) - m22);
				var num3 = 0.5f / num6;
				quaternion.x = (m10 + m01) * num3;
				quaternion.y = 0.5f * num6;
				quaternion.z = (m21 + m12) * num3;
				quaternion.w = (m20 - m02) * num3;
				return quaternion;
			}
			var num5 = (float)Math.Sqrt(((1f + m22) - m00) - m11);
			var num2 = 0.5f / num5;
			quaternion.x = (m20 + m02) * num2;
			quaternion.y = (m21 + m12) * num2;
			quaternion.z = 0.5f * num5;
			quaternion.w = (m01 - m10) * num2;
			return quaternion;
		}
		#endregion

		#region Double Implementation
		/// <summary>
		///   <para>Creates a rotation with the specified forward and upwards directions.</para>
		/// </summary>
		/// <param name="forward">The direction to look in.</param>
		/// <param name="upwards">The vector that defines in which direction up is.</param>
		public static Quaternion<double> LookRotation(this Vector3<double> forward, [DefaultValue("Vector3<double>.up")] Vector3<double> upwards)
		{
			return INTERNAL_CALL_LookRotation(ref forward, ref upwards);
		}

		/// <summary>
		///   <para>Creates a rotation with the specified forward and upwards directions.</para>
		/// </summary>
		/// <param name="forward">The direction to look in.</param>
		/// <param name="upwards">The vector that defines in which direction up is.</param>
		public static Quaternion<double> LookRotation(this Vector3<double> forward)
		{
			Vector3<double> up = Vector3<double>.up;
			return INTERNAL_CALL_LookRotation(ref forward, ref up);
		}

		//Form: https://gist.github.com/HelloKitty/91b7af87aac6796c3da9
		//and http://answers.unity3d.com/questions/467614/what-is-the-source-code-of-quaternionlookrotation.html
		private static Quaternion<double> INTERNAL_CALL_LookRotation(ref Vector3<double> forward, ref Vector3<double> up)
		{

			forward = forward.Normalized();
			Vector3<double> right = up.Cross(forward).Normalized();
			up = forward.Cross(right);
			var m00 = right.x;
			var m01 = right.y;
			var m02 = right.z;
			var m10 = up.x;
			var m11 = up.y;
			var m12 = up.z;
			var m20 = forward.x;
			var m21 = forward.y;
			var m22 = forward.z;


			double num8 = (m00 + m11) + m22;
			var quaternion = new Quaternion<double>();
			if (num8 > 0d)
			{
				var num = Math.Sqrt(num8 + 1d);
				quaternion.w = num * 0.5d;
				num = 0.5d / num;
				quaternion.x = (m12 - m21) * num;
				quaternion.y = (m20 - m02) * num;
				quaternion.z = (m01 - m10) * num;
				return quaternion;
			}
			if ((m00 >= m11) && (m00 >= m22))
			{
				var num7 = Math.Sqrt(((1d + m00) - m11) - m22);
				var num4 = 0.5d / num7;
				quaternion.x = 0.5d * num7;
				quaternion.y = (m01 + m10) * num4;
				quaternion.z = (m02 + m20) * num4;
				quaternion.w = (m12 - m21) * num4;
				return quaternion;
			}
			if (m11 > m22)
			{
				var num6 = Math.Sqrt(((1d + m11) - m00) - m22);
				var num3 = 0.5d / num6;
				quaternion.x = (m10 + m01) * num3;
				quaternion.y = 0.5d * num6;
				quaternion.z = (m21 + m12) * num3;
				quaternion.w = (m20 - m02) * num3;
				return quaternion;
			}
			var num5 = Math.Sqrt(((1d + m22) - m00) - m11);
			var num2 = 0.5d / num5;
			quaternion.x = (m20 + m02) * num2;
			quaternion.y = (m21 + m12) * num2;
			quaternion.z = 0.5d * num5;
			quaternion.w = (m01 - m10) * num2;
			return quaternion;
		}
		#endregion

		#endregion

		#region FromToRotation
		/// <summary>
		///   <para>Creates a rotation which rotates from /fromDirection/ to /toDirection/.</para>
		/// </summary>
		/// <param name="fromDirection"></param>
		/// <param name="toDirection"></param>
		public static Quaternion<float> FromToRotation(Vector3<float> fromDirection, Vector3<float> toDirection)
		{
			return RotateTowards(fromDirection.LookRotation(), toDirection.LookRotation(), float.MaxValue);
		}

		/// <summary>
		///   <para>Creates a rotation which rotates from /fromDirection/ to /toDirection/.</para>
		/// </summary>
		/// <param name="fromDirection"></param>
		/// <param name="toDirection"></param>
		public static Quaternion<double> FromToRotation(Vector3<double> fromDirection, Vector3<double> toDirection)
		{
			return RotateTowards(fromDirection.LookRotation(), toDirection.LookRotation(), double.MaxValue);
		}
		#endregion

		#region AngleAxis

		#region Float Implementation

		/// <summary>
		///   <para>Creates a rotation which rotates angle degrees around axis.</para>
		/// </summary>
		/// <param name="angle"></param>
		/// <param name="axis"></param>
		public static Quaternion<float> AngleAxis(this Vector3<float> axis, float angle)
		{
			return INTERNAL_CALL_AngleAxis(angle, ref axis);
		}

		private static Quaternion<float> INTERNAL_CALL_AngleAxis(float degress, ref Vector3<float> axis)
		{
			if (axis.SqrMagnitude() == 0.0f)
				return Quaternion<float>.identity;

			Quaternion<float> result = Quaternion<float>.identity;
			var radians = degress * degToRadFloat;
			radians *= 0.5f;
			axis = axis.Normalized();
			axis = axis * (float)System.Math.Sin(radians);
			result.x = axis.x;
			result.y = axis.y;
			result.z = axis.z;
			result.w = (float)System.Math.Cos(radians);

			return Normalize(result);
		}

		#endregion

		#region Double Implementation
		/// <summary>
		///   <para>Creates a rotation which rotates angle degrees around axis.</para>
		/// </summary>
		/// <param name="angle"></param>
		/// <param name="axis"></param>
		public static Quaternion<double> AngleAxis(this Vector3<double> axis, double angle)
		{
			return INTERNAL_CALL_AngleAxis(angle, ref axis);
		}

		private static Quaternion<double> INTERNAL_CALL_AngleAxis(double degress, ref Vector3<double> axis)
		{
			if (axis.SqrMagnitude() == 0.0f)
				return Quaternion<double>.identity;

			Quaternion<double> result = Quaternion<double>.identity;
			var radians = degress * degToRadDouble;
			radians *= 0.5f;
			axis = axis.Normalized();
			axis = axis * (double)System.Math.Sin(radians);
			result.x = axis.x;
			result.y = axis.y;
			result.z = axis.z;
			result.w = (double)System.Math.Cos(radians);

			return Normalize(result);
		}
		#endregion

		#endregion

		//Originally: SetFromToRotation. Creates a quat from two vector3 rotations.
		#region QuatFromToRotation
		/// <summary>
		///   <para>Creates a rotation which rotates from fromDirection to toDirection.</para>
		/// </summary>
		/// <param name="fromDirection"></param>
		/// <param name="toDirection"></param>
		public static Quaternion<float> QuatFromToRotation(this Vector3<float> fromDirection, Vector3<float> toDirection)
		{
			return FromToRotation(fromDirection, toDirection);
		}

		/// <summary>
		///   <para>Creates a rotation which rotates from fromDirection to toDirection.</para>
		/// </summary>
		/// <param name="fromDirection"></param>
		/// <param name="toDirection"></param>
		public static Quaternion<double> QuatFromToRotation(this Vector3<double> fromDirection, Vector3<double> toDirection)
		{
			return FromToRotation(fromDirection, toDirection);
		}
		#endregion

		//Originally: SetLookRotation. Creates a Quat from a Vector3 rotation.
		#region QuatLookRotation

		#region Float Implementation
		/// <summary>
		///   <para>Creates a rotation with the specified forward and upwards directions.</para>
		/// </summary>
		/// <param name="view">The direction to look in.</param>
		/// <param name="up">The vector that defines in which direction up is.</param>
		public static Quaternion<float> QuatLookRotation(this Vector3<float> view)
		{
			return QuatLookRotation(view, Vector3<float>.up);
		}

		/// <summary>
		///   <para>Creates a rotation with the specified forward and upwards directions.</para>
		/// </summary>
		/// <param name="view">The direction to look in.</param>
		/// <param name="up">The vector that defines in which direction up is.</param>
		public static Quaternion<float> QuatLookRotation(this Vector3<float> view, [DefaultValue("Vector3<float>.up")] Vector3<float> up)
		{
			return LookRotation(view, up);
		}
		#endregion

		#region Double Implementation
		/// <summary>
		///   <para>Creates a rotation with the specified forward and upwards directions.</para>
		/// </summary>
		/// <param name="view">The direction to look in.</param>
		/// <param name="up">The vector that defines in which direction up is.</param>
		public static Quaternion<double> QuatLookRotation(this Vector3<double> view)
		{
			return QuatLookRotation(view, Vector3<double>.up);
		}

		/// <summary>
		///   <para>Creates a rotation with the specified forward and upwards directions.</para>
		/// </summary>
		/// <param name="view">The direction to look in.</param>
		/// <param name="up">The vector that defines in which direction up is.</param>
		public static Quaternion<double> QuatLookRotation(this Vector3<double> view, [DefaultValue("Vector3<double>.up")] Vector3<double> up)
		{
			return LookRotation(view, up);
		}
		#endregion

		#endregion

		#region ToAngleAxis

		#region Float Implementation
		public static void ToAngleAxis(this Quaternion<float> quat, out float angle, out Vector3<float> axis)
		{
			Internal_ToAxisAngleRad(ref quat, out axis, out angle);
			angle *= radToDegFloat;
		}

		private static void Internal_ToAxisAngleRad(ref Quaternion<float> q, out Vector3<float> axis, out float angle)
		{
			if (Math.Abs(q.w) > 1.0f)
				q.Normalize();


			angle = 2.0f * (float)System.Math.Acos(q.w); // angle
			float den = (float)System.Math.Sqrt(1.0 - q.w * q.w);
			if (den > 0.0001f)
			{
				axis = q.xyz * (1.0f / den);
			}
			else
			{
				// This occurs when the angle is zero. 
				// Not a problem: just set an arbitrary normalized axis.
				axis = new Vector3<float>(1, 0, 0);
			}
		}
		#endregion

		#region Double Implementation
		public static void ToAngleAxis(this Quaternion<double> quat, out double angle, out Vector3<double> axis)
		{
			Internal_ToAxisAngleRad(ref quat, out axis, out angle);
			angle *= radToDegDouble;
		}

		private static void Internal_ToAxisAngleRad(ref Quaternion<double> q, out Vector3<double> axis, out double angle)
		{
			if (Math.Abs(q.w) > 1.0f)
				q.Normalize();


			angle = 2.0f * (double)System.Math.Acos(q.w); // angle
			double den = (double)System.Math.Sqrt(1.0 - q.w * q.w);
			if (den > 0.0001f)
			{
				axis = q.xyz * (1.0f / den);
			}
			else
			{
				// This occurs when the angle is zero. 
				// Not a problem: just set an arbitrary normalized axis.
				axis = new Vector3<double>(1, 0, 0);
			}
		}
		#endregion

		#endregion

		//Originally: * operator with lhs quat and rhs vec3. Edit: Even after the move JIT produces different code. Probably handles operators differently.
		#region RotateVector
		
		//See the UnityEngine.dll decompliled code. It's what this ugly stuff is based on.

		/// <summary>
		/// Rotates a <see cref="Vector3{TMathType}"/> by the given <see cref="Quaternion{TMathType}"/> rotation.
		/// </summary>
		/// <param name="rotation">Rotation to rotate the <see cref="Vector3{TMathType}"/> by.</param>
		/// <param name="point"><see cref="Vector3{TMathType}"/> to rotate.</param>
		/// <returns>A <see cref="Vector3{TMathType}"/> rotated by the <see cref="Quaternion{TMathType}"</see>.</returns>
		public static Vector3<float> RotateVector(this Quaternion<float> rotation, Vector3<float> point)
		{
			//From UnityEngine.dll; decompiled with JustDecompile.
			Vector3<float> vector3 = new Vector3<float>();
			float single = rotation.x * 2f;
			float single1 = rotation.y * 2f;
			float single2 = rotation.z * 2f;
			float single3 = rotation.x * single;
			float single4 = rotation.y * single1;
			float single5 = rotation.z * single2;
			float single6 = rotation.x * single1;
			float single7 = rotation.x * single2;
			float single8 = rotation.y * single2;
			float single9 = rotation.w * single;
			float single10 = rotation.w * single1;
			float single11 = rotation.w * single2;
			vector3.x = (1f - (single4 + single5)) * point.x + (single6 - single11) * point.y + (single7 + single10) * point.z;
			vector3.y = (single6 + single11) * point.x + (1f - (single3 + single5)) * point.y + (single8 - single9) * point.z;
			vector3.z = (single7 - single10) * point.x + (single8 + single9) * point.y + (1f - (single3 + single4)) * point.z;
			return vector3;
		}

		/// <summary>
		/// Rotates a <see cref="Vector3{TMathType}"/> by the given <see cref="Quaternion{TMathType}"/> rotation.
		/// </summary>
		/// <param name="rotation">Rotation to rotate the <see cref="Vector3{TMathType}"/> by.</param>
		/// <param name="point"><see cref="Vector3{TMathType}"/> to rotate.</param>
		/// <returns>A <see cref="Vector3{TMathType}"/> rotated by the <see cref="Quaternion{TMathType}"</see>.</returns>
		public static Vector3<double> RotateVector(this Quaternion<double> rotation, Vector3<double> point)
		{
			//From UnityEngine.dll; decompiled with JustDecompile.
			Vector3<double> vector3 = new Vector3<double>();
			double doubleTerm = rotation.x * 2d;
			double doubleTerm1 = rotation.y * 2d;
			double doubleTerm2 = rotation.z * 2d;
			double doubleTerm3 = rotation.x * doubleTerm;
			double doubleTerm4 = rotation.y * doubleTerm1;
			double doubleTerm5 = rotation.z * doubleTerm2;
			double doubleTerm6 = rotation.x * doubleTerm1;
			double doubleTerm7 = rotation.x * doubleTerm2;
			double doubleTerm8 = rotation.y * doubleTerm2;
			double doubleTerm9 = rotation.w * doubleTerm;
			double doubleTerm10 = rotation.w * doubleTerm1;
			double doubleTerm11 = rotation.w * doubleTerm2;
			vector3.x = (1f - (doubleTerm4 + doubleTerm5)) * point.x + (doubleTerm6 - doubleTerm11) * point.y + (doubleTerm7 + doubleTerm10) * point.z;
			vector3.y = (doubleTerm6 + doubleTerm11) * point.x + (1f - (doubleTerm3 + doubleTerm5)) * point.y + (doubleTerm8 - doubleTerm9) * point.z;
			vector3.z = (doubleTerm7 - doubleTerm10) * point.x + (doubleTerm8 + doubleTerm9) * point.y + (1f - (doubleTerm3 + doubleTerm4)) * point.z;
			return vector3;
		}
		#endregion

		//Originally: * operator with lhs quat and rhs quat.
		#region MultiplyBy
		public static Quaternion<float> MultiplyBy(this Quaternion<float> lhs, Quaternion<float> rhs)
		{
			return new Quaternion<float>(lhs.w * rhs.x + lhs.x * rhs.w + lhs.y * rhs.z - lhs.z * rhs.y, lhs.w * rhs.y + lhs.y * rhs.w + lhs.z * rhs.x - lhs.x * rhs.z, lhs.w * rhs.z + lhs.z * rhs.w + lhs.x * rhs.y - lhs.y * rhs.x, lhs.w * rhs.w - lhs.x * rhs.x - lhs.y * rhs.y - lhs.z * rhs.z);
		}

		public static Quaternion<double> MultiplyBy(this Quaternion<double> lhs, Quaternion<double> rhs)
		{
			return new Quaternion<double>(lhs.w * rhs.x + lhs.x * rhs.w + lhs.y * rhs.z - lhs.z * rhs.y, lhs.w * rhs.y + lhs.y * rhs.w + lhs.z * rhs.x - lhs.x * rhs.z, lhs.w * rhs.z + lhs.z * rhs.w + lhs.x * rhs.y - lhs.y * rhs.x, lhs.w * rhs.w - lhs.x * rhs.x - lhs.y * rhs.y - lhs.z * rhs.z);
		}
		#endregion
	}
}
