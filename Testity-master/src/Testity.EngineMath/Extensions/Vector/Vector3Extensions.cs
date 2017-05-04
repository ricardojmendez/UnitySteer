using MiscUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Testity.EngineMath
{
	public static class Vector3Extensions
	{
		#region Cross Products
		//This class mostly exists because users can't expect all types of Vector3<T> to offers certain
		//functionality. The best way to seperate it into valid types is to use extension methods
		public static Vector3<float> Cross(this Vector3<float> lhs, Vector3<float> rhs)
		{
			return new Vector3<float>(lhs.y * rhs.z - lhs.z * rhs.y, lhs.z * rhs.x - lhs.x * rhs.z, lhs.x * rhs.y - lhs.y * rhs.x);
		}

		//This class mostly exists because users can't expect all types of Vector3<T> to offers certain
		//functionality. The best way to seperate it into valid types is to use extension methods
		public static Vector3<double> Cross(this Vector3<double> lhs, Vector3<double> rhs)
		{
			return new Vector3<double>(lhs.y * rhs.z - lhs.z * rhs.y, lhs.z * rhs.x - lhs.x * rhs.z, lhs.x * rhs.y - lhs.y * rhs.x);
		}
		#endregion

		#region Normalization
		/// <summary>
		///   <para>Normalizes the vector to the best precision allowed by <see cref="Vector3{Double}"/>.</para>
		/// Only some Vector types have valid normalizations
		/// </summary>
		public static Vector3<double> Normalized(this Vector3<double> vec)
		{
			double single = vec.Magnitude();

			if (single <= Vector3<double>.kEpsilon)
				return Vector3<double>.zero;
			else
				return vec * (1.0d / single);
		}

		/// <summary>
		///   <para>Normalizes the vector to the best precision allowed by <see cref="Vector3{float}"/>.</para>
		/// Only some Vector types have valid normalizations
		/// /// </summary>
		public static Vector3<float> Normalized(this Vector3<float> vec)
		{
			float single = vec.Magnitude();

			if (single <= Vector3<float>.kEpsilon)
				return Vector3<float>.zero;
			else
				return vec * (1.0f / single);
		}
		#endregion

		#region Double ClampMagnitude
		/// <summary>
		///   <para>Returns a copy of vector with its magnitude clamped to maxLength.</para>
		/// </summary>
		/// <param name="vector">Vector whose magnitude should be clamped.</param>
		/// <param name="maxLength">Value to clamp by.</param>
		public static Vector3<double> ClampMagnitude(this Vector3<double> vector, double maxLength)
		{
			//TODO: Check if this type will provide value results.
			if (vector.SqrMagnitude() <= (maxLength * maxLength))
			{
				return vector;
			}

			return vector.Normalized() * maxLength;
		}

		/// <summary>
		///   <para>Returns a copy of vector with its magnitude clamped to maxLength.</para>
		/// </summary>
		/// <param name="vector">Vector whose magnitude should be clamped.</param>
		/// <param name="maxLength">Value to clamp by.</param>
		public static Vector3<double> ClampMagnitude(this Vector3<double> vector, float maxLength)
		{
			//TODO: Check if this type will provide value results.
			if (vector.SqrMagnitude() <= (maxLength * maxLength))
			{
				return vector;
			}

			return vector.Normalized() * maxLength;
		}
		#endregion

		#region Float ClampMagnitude
		/// <summary>
		///   <para>Returns a copy of vector with its magnitude clamped to maxLength.</para>
		/// </summary>
		/// <param name="vector">Vector whose magnitude should be clamped.</param>
		/// <param name="maxLength">Value to clamp by.</param>
		public static Vector3<float> ClampMagnitude(this Vector3<float> vector, double maxLength)
		{
			//TODO: Check if this type will provide value results.
			if (vector.SqrMagnitude() <= (maxLength * maxLength))
			{
				return vector;
			}

			return vector.Normalized() * (float)maxLength;
		}

		/// <summary>
		///   <para>Returns a copy of vector with its magnitude clamped to maxLength.</para>
		/// </summary>
		/// <param name="vector">Vector whose magnitude should be clamped.</param>
		/// <param name="maxLength">Value to clamp by.</param>
		public static Vector3<float> ClampMagnitude(this Vector3<float> vector, float maxLength)
		{
			//TODO: Check if this type will provide value results.
			if (vector.SqrMagnitude() <= (maxLength * maxLength))
			{
				return vector;
			}

			return vector.Normalized() * maxLength;
		}
		#endregion

		#region Projections

		#region Double Projections
		/// <summary>
		///   <para>Projects a vector onto another vector.</para>
		/// </summary>
		/// <param name="vector"></param>
		/// <param name="onNormal"></param>
		public static Vector3<double> Project(this Vector3<double> vector, Vector3<double> onNormal)
		{
			double single = onNormal.Dot(onNormal);

			if (single <= Vector3<double>.kEpsilon)
			{
				return Vector3<double>.zero;
			}

			return (onNormal * vector.Dot(onNormal)) * (1d / single);
		}

		/// <summary>
		///   <para>Projects a vector onto a plane defined by a normal orthogonal to the plane.</para>
		/// </summary>
		/// <param name="vector"></param>
		/// <param name="planeNormal"></param>
		public static Vector3<double> ProjectOnPlane(this Vector3<double> vector, Vector3<double> planeNormal)
		{
			//return ProjectOnPlaneInternalGeneric(vector, planeNormal);
			return vector - vector.Project(planeNormal);
		}
		#endregion

		#region Float Projections
		/// <summary>
		///   <para>Projects a vector onto another vector.</para>
		/// </summary>
		/// <param name="vector"></param>
		/// <param name="onNormal"></param>
		public static Vector3<float> Project(this Vector3<float> vector, Vector3<float> onNormal)
		{
			//return ProjectInternalGeneric(vector, onNormal);
			float single = onNormal.Dot(onNormal);

			if (single < Vector3<float>.kEpsilon)
			{
				return Vector3<float>.zero;
			}

			return (onNormal * vector.Dot(onNormal)) * (1.0f / single);
		}

		/// <summary>
		///   <para>Projects a vector onto a plane defined by a normal orthogonal to the plane.</para>
		/// </summary>
		/// <param name="vector"></param>
		/// <param name="planeNormal"></param>
		public static Vector3<float> ProjectOnPlane(this Vector3<float> vector, Vector3<float> planeNormal)
		{
			return vector - vector.Project(planeNormal);
		}
		#endregion
		#endregion

		#region Magnitude Methods
		public static float Magnitude(this Vector3<float> vector)
		{
			//This isn't as accurate as it could be. Maybe provide a secondary more accurate method.
			return (float)Math.Sqrt(vector.SqrMagnitude());
		}

		public static double Magnitude(this Vector3<double> vector)
		{
			//This isn't as accurate as it could be. Maybe provide a secondary more accurate method.
			return (double)Math.Sqrt(vector.SqrMagnitude());
		}

		public static float Magnitude(this Vector3<int> vector)
		{
			//This isn't as accurate as it could be. Maybe provide a secondary more accurate method.
			return (float)Math.Sqrt(vector.SqrMagnitude());
		}

		/// <summary>
		/// Returns the non-square rooted magnitude of the vector.
		/// </summary>
		/// <returns>Non-rooted magnitude.</returns>
		public static float SqrMagnitude(this Vector3<float> vector)
		{
			return vector.x * vector.x + vector.y * vector.y + vector.z * vector.z;
		}

		/// <summary>
		/// Returns the non-square rooted magnitude of the vector.
		/// </summary>
		/// <returns>Non-rooted magnitude.</returns>
		public static double SqrMagnitude(this Vector3<double> vector)
		{
			return vector.x * vector.x + vector.y * vector.y + vector.z * vector.z;
		}

		/// <summary>
		/// Returns the non-square rooted magnitude of the vector.
		/// </summary>
		/// <returns>Non-rooted magnitude.</returns>
		public static float SqrMagnitude(this Vector3<int> vector)
		{
			return vector.x * vector.x + vector.y * vector.y + vector.z * vector.z;
		}
		#endregion

		#region Distance Methods
		/// <summary>
		///   <para>Returns the distance between a and b.</para>
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		public static float Distance(Vector3<float> a, Vector3<float> b)
		{
			Vector3<float> vec3 = new Vector3<float>(a.x - b.x, a.y - b.y, a.z - b.z);
			return vec3.Magnitude();
		}

		/// <summary>
		///   <para>Returns the distance between a and b.</para>
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		public static double Distance(Vector3<double> a, Vector3<double> b)
		{
			Vector3<double> vec3 = new Vector3<double>(a.x - b.x, a.y - b.y, a.z - b.z);
			return vec3.Magnitude();
		}

		/// <summary>
		///   <para>Returns the distance between a and b.</para>
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		public static float Distance(Vector3<int> a, Vector3<int> b)
		{
			Vector3<int> vec3 = new Vector3<int>(a.x - b.x, a.y - b.y, a.z - b.z);
			return vec3.Magnitude();
		}
		#endregion

		#region Dot Products
		/// <summary>
		///   <para>Dot Product of two vectors.</para>
		/// </summary>
		/// <param name="lhs"></param>
		/// <param name="rhs"></param>
		public static float Dot(this Vector3<float> lhs, Vector3<float> rhs)
		{
			return lhs.x * rhs.x + lhs.y * rhs.y + lhs.z * rhs.z;
		}

		/// <summary>
		///   <para>Dot Product of two vectors.</para>
		/// </summary>
		/// <param name="lhs"></param>
		/// <param name="rhs"></param>
		public static double Dot(this Vector3<double> lhs, Vector3<double> rhs)
		{
			return lhs.x * rhs.x + lhs.y * rhs.y + lhs.z * rhs.z;
		}

		/// <summary>
		///   <para>Dot Product of two vectors.</para>
		/// </summary>
		/// <param name="lhs"></param>
		/// <param name="rhs"></param>
		public static int Dot(this Vector3<int> lhs, Vector3<int> rhs)
		{
			return lhs.x * rhs.x + lhs.y * rhs.y + lhs.z * rhs.z;
		}

		/// <summary>
		///   <para>Dot Product of two vectors.</para>
		/// </summary>
		/// <param name="lhs"></param>
		/// <param name="rhs"></param>
		public static char Dot(this Vector3<char> lhs, Vector3<char> rhs)
		{
			return (char)((int)lhs.x * (int)rhs.x + (int)lhs.y * (int)rhs.y + (int)lhs.z * (int)rhs.z);
		}

		/// <summary>
		///   <para>Dot Product of two vectors.</para>
		/// </summary>
		/// <param name="lhs"></param>
		/// <param name="rhs"></param>
		public static byte Dot(this Vector3<byte> lhs, Vector3<byte> rhs)
		{
			return (byte)((int)lhs.x * (int)rhs.x + (int)lhs.y * (int)rhs.y + (int)lhs.z * (int)rhs.z);
		}
		#endregion

		#region Reflect Methods
		/// <summary>
		///   <para>Reflects a vector off the plane defined by a normal.</para>
		/// </summary>
		/// <param name="inDirection"></param>
		/// <param name="inNormal"></param>
		public static Vector3<float> Reflect(this Vector3<float> inDirection, Vector3<float> inNormal)
		{
			//In case of ints it is -2 * some int (which is int). Dot product is closed under ints so this is valid for ints.
			float twoTimesDotNDir = -2.0f * inNormal.Dot(inDirection);
			//this operation is also closed for ints. We do not need to make an extension method.
			return (twoTimesDotNDir * inNormal) + inDirection;
		}

		/// <summary>
		///   <para>Reflects a vector off the plane defined by a normal.</para>
		/// </summary>
		/// <param name="inDirection"></param>
		/// <param name="inNormal"></param>
		public static Vector3<double> Reflect(this Vector3<double> inDirection, Vector3<double> inNormal)
		{
			//In case of ints it is -2 * some int (which is int). Dot product is closed under ints so this is valid for ints.
			double twoTimesDotNDir = -2.0f * inNormal.Dot(inDirection);
			//this operation is also closed for ints. We do not need to make an extension method.
			return (twoTimesDotNDir * inNormal) + inDirection;
		}

		/// <summary>
		///   <para>Reflects a vector off the plane defined by a normal.</para>
		/// </summary>
		/// <param name="inDirection"></param>
		/// <param name="inNormal"></param>
		public static Vector3<int> Reflect(this Vector3<int> inDirection, Vector3<int> inNormal)
		{
			//In case of ints it is -2 * some int (which is int). Dot product is closed under ints so this is valid for ints.
			int twoTimesDotNDir = -2 * inNormal.Dot(inDirection);
			//this operation is also closed for ints. We do not need to make an extension method.
			return (twoTimesDotNDir * inNormal) + inDirection;
		}

		/// <summary>
		///   <para>Reflects a vector off the plane defined by a normal.</para>
		/// </summary>
		/// <param name="inDirection"></param>
		/// <param name="inNormal"></param>
		public static Vector3<byte> Reflect(this Vector3<byte> inDirection, Vector3<byte> inNormal)
		{
			//In case of bytes it is -2 * some byte (which is byte). Dot product is closed under bytes so this is valid for bytes.
			byte twoTimesDotNDir = (byte)(-2 * (int)inNormal.Dot(inDirection));
			//this operation is also closed for bytes. We do not need to make an extension method.
			return (twoTimesDotNDir * inNormal) + inDirection;
		}
		#endregion

		#region Scale
		/// <summary>
		///   <para>Multiplies two vectors component-wise.</para>
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		public static Vector3<float> Scale(this Vector3<float> a, Vector3<float> b)
		{
			return new Vector3<float>(a.x * b.x, a.y * b.y, a.z * b.z);
		}

		/// <summary>
		///   <para>Multiplies two vectors component-wise.</para>
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		public static Vector3<double> Scale(this Vector3<double> a, Vector3<double> b)
		{
			return new Vector3<double>(a.x * b.x, a.y * b.y, a.z * b.z);
		}

		/// <summary>
		///   <para>Multiplies two vectors component-wise.</para>
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		public static Vector3<int> Scale(this Vector3<int> a, Vector3<int> b)
		{
			return new Vector3<int>(a.x * b.x, a.y * b.y, a.z * b.z);
		}

		/// <summary>
		///   <para>Multiplies two vectors component-wise.</para>
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		public static Vector3<byte> Scale(this Vector3<byte> a, Vector3<byte> b)
		{
			return new Vector3<byte>((byte)((int)a.x * (int)b.x), (byte)((int)a.y * (int)b.y), (byte)((int)a.z * (int)b.z));
		}
		#endregion
	}
}
