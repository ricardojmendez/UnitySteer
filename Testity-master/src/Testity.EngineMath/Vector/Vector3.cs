using MiscUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Testity.EngineMath;
using System.Collections;
using Testity.Common;

namespace Testity.EngineMath
{
	//For information on how and why this is efficient see: http://stackoverflow.com/questions/1348594/is-there-a-c-sharp-generic-constraint-for-real-number-types
	//A library by the legendary Jon Skeet and Marc Gravell cannot be dismissed. It works.
	/// <summary>
	/// Generic vector type with 3 components. Allows for use of int, float and double types.
	/// Based on Unity3D's: http://docs.unity3d.com/ScriptReference/Vector3.html
	/// </summary>
	/// <typeparam name="TMathType">Value type of the vector that must overload math operators (Ex. int, float, double).</typeparam>
	[EngineSerializableMapsToType(EngineType.Unity3D, "UnityEngine.Vector3, UnityEngine")]
	public struct Vector3<TMathType> : IEquatable<Vector3<TMathType>>, IEnumerable<TMathType>
		where TMathType : struct, IEquatable<TMathType>, IComparable<TMathType>
	{
		public static TMathType kEpsilon = MathT.GenerateKEpslion<TMathType>();

		private static TMathType validCompareError = ValidCompareErrorGenerator();

		private static TMathType ValidCompareErrorGenerator()
		{
			try
			{
				return (TMathType)Convert.ChangeType(9.99999944E-11f, typeof(TMathType));
			}
			catch (InvalidCastException)
			{
#if DEBUG || DEBUGBUILD
				//These are known types that cause issues with kEpsilon
				if (typeof(TMathType) != typeof(char))
					throw;
#endif
				return Operator<TMathType>.Zero;
			}

		}

		/// <summary>
		///   <para>X component of the vector.</para>
		/// </summary>
		public TMathType x;

		/// <summary>
		///   <para>Y component of the vector.</para>
		/// </summary>
		public TMathType y;

		/// <summary>
		///   <para>Z component of the vector.</para>
		/// </summary>
		public TMathType z;

		/// <summary>
		/// A cache of the <see cref="TMathType"/> value that represents 1.
		/// </summary>
		private static TMathType oneValue = MathT.GenerateOneValue<TMathType>();

		/// <summary>
		///   <para>Shorthand for writing Vector3<TMathType>(0, 0, -1).</para>
		/// </summary>
		public static Vector3<TMathType> back
		{
			get
			{
				return new Vector3<TMathType>(Operator<TMathType>.Zero, Operator<TMathType>.Zero,
					Operator.Subtract(Operator<TMathType>.Zero, Vector3<TMathType>.oneValue));
			}
		}

		/// <summary>
		///   <para>Shorthand for writing Vector3<TMathType>(0, -1, 0).</para>
		/// </summary>
		public static Vector3<TMathType> down
		{
			get
			{
				return new Vector3<TMathType>(Operator<TMathType>.Zero, Operator.Subtract(Operator<TMathType>.Zero, Vector3<TMathType>.oneValue),
					Operator<TMathType>.Zero);
			}
		}

		/// <summary>
		///   <para>Shorthand for writing Vector3<TMathType>(0, 0, 1).</para>
		/// </summary>
		public static Vector3<TMathType> forward
		{
			get
			{
				return new Vector3<TMathType>(Operator<TMathType>.Zero, Operator<TMathType>.Zero, Vector3<TMathType>.oneValue);
			}
		}

		/// <summary>
		///   <para>Shorthand for writing Vector3<TMathType>(-1, 0, 0).</para>
		/// </summary>
		public static Vector3<TMathType> left
		{
			get
			{
				return new Vector3<TMathType>(Operator.Subtract(Operator<TMathType>.Zero, Vector3<TMathType>.oneValue), Operator<TMathType>.Zero,
					Operator<TMathType>.Zero);
			}
		}

		/// <summary>
		///   <para>Shorthand for writing Vector3<TMathType>(1, 1, 1).</para>
		/// </summary>
		public static Vector3<TMathType> one
		{
			get
			{
				return new Vector3<TMathType>(Vector3<TMathType>.oneValue, Vector3<TMathType>.oneValue, Vector3<TMathType>.oneValue);
			}
		}

		/// <summary>
		///   <para>Shorthand for writing Vector3<TMathType>(1, 0, 0).</para>
		/// </summary>
		public static Vector3<TMathType> right
		{
			get
			{
				return new Vector3<TMathType>(Vector3<TMathType>.oneValue, Operator<TMathType>.Zero, Operator<TMathType>.Zero);

			}
		}

		/// <summary>
		///   <para>Shorthand for writing Vector3<TMathType>(0, 1, 0).</para>
		/// </summary>
		public static Vector3<TMathType> up
		{
			get
			{
				return new Vector3<TMathType>(Operator<TMathType>.Zero, Vector3<TMathType>.oneValue, Operator<TMathType>.Zero);
			}
		}

		/// <summary>
		///   <para>Shorthand for writing Vector3<TMathType>(0, 0, 0).</para>
		/// </summary>
		public static Vector3<TMathType> zero
		{
			get
			{
				return new Vector3<TMathType>(Operator<TMathType>.Zero, Operator<TMathType>.Zero, Operator<TMathType>.Zero);
			}
		}

		public TMathType this[int index]
		{
			get
			{
				switch (index)
				{
					case 0:
						return this.x;
					case 1:
						return this.y;
					case 2:
						return this.z;
				}
				throw new IndexOutOfRangeException("Invalid " + nameof(Vector3<TMathType>) + " index!");
			}
			set
			{
				switch (index)
				{
					case 0:
						this.x = value;
						break;
					case 1:
						this.y = value;
						break;
					case 2:
						this.z = value;
						break;
					default:
						throw new IndexOutOfRangeException("Invalid " + nameof(Vector3<TMathType>) + " index!");
				}
			}
		}

		/// <summary>
		///   <para>Creates a new vector with given x, y, z components.</para>
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		public Vector3(TMathType x, TMathType y, TMathType z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		/// <summary>
		///   <para>Creates a new vector with given x, y components and sets z to zero.</para>
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public Vector3(TMathType x, TMathType y)
			: this(x, y, Operator<TMathType>.Zero)
		{

		}

		public override bool Equals(object other)
		{
			if (!(other is Vector3<TMathType>))
			{
				return false;
			}

			Vector3<TMathType> Vector3 = (Vector3<TMathType>)other;
			return this.Equals(Vector3); //calls generic version
		}

		public bool Equals(Vector3<TMathType> other)
		{
			return (!this.x.Equals(other.x) || !this.y.Equals(other.y) ? false : this.z.Equals(other.z));
		}

		//No idea what is going on here... This is Unity3D's decompiled GetHashCode implementation.
		public override int GetHashCode()
		{
			return this.x.GetHashCode() ^ this.y.GetHashCode() << 2 ^ this.z.GetHashCode() >> 2;
		}

		/// <summary>
		///   <para>Returns a vector that is made from the largest components of two vectors.</para>
		/// </summary>
		/// <param name="lhs"></param>
		/// <param name="rhs"></param>
		public static Vector3<TMathType> Max(Vector3<TMathType> lhs, Vector3<TMathType> rhs)
		{
			return new Vector3<TMathType>(MathT.Max(lhs.x, rhs.x), MathT.Max(lhs.y, rhs.y), MathT.Max(lhs.z, rhs.z));
		}

		/// <summary>
		///   <para>Returns a vector that is made from the smallest components of two vectors.</para>
		/// </summary>
		/// <param name="lhs"></param>
		/// <param name="rhs"></param>
		public static Vector3<TMathType> Min(Vector3<TMathType> lhs, Vector3<TMathType> rhs)
		{
			return new Vector3<TMathType>(MathT.Min(lhs.x, rhs.x), MathT.Min(lhs.y, rhs.y), MathT.Min(lhs.z, rhs.z));
		}

		#region Operator Overloads
		public static Vector3<TMathType> operator +(Vector3<TMathType> a, Vector3<TMathType> b)
		{
			return new Vector3<TMathType>(Operator.Add(a.x, b.x), Operator.Add(a.y, b.y),
				Operator.Add(a.z, b.z));
		}

		public static bool operator ==(Vector3<TMathType> lhs, Vector3<TMathType> rhs)
		{
			//Do componentwise comaparision instead since it's more reliable
			for (int i = 0; i < 3; i++)
				if (!lhs[i].Equals(rhs[i]))
					return false;

			return true;
			//Don't do it like this. We can't be 100% sure it will work for all types.
			//return Operator.LessThan(Vector3<TMathType>.SqrMagnitude<TMathType>(lhs - rhs), Vector3<TMathType>.validCompareError);
		}

		public static bool operator !=(Vector3<TMathType> lhs, Vector3<TMathType> rhs)
		{
			return !(lhs == rhs);
			//Don't do it like this. We can't be 100% sure it will work for all types.
			//return Operator.GreaterThanOrEqual(Vector3<TMathType>.SqrMagnitude(lhs - rhs), Vector3<TMathType>.validCompareError);
		}

		/// <summary>
		/// Scales the components of the vector by d.
		/// </summary>
		/// <param name="a">Vector to scale.</param>
		/// <param name="d">cale</param>
		/// <returns></returns>
		public static Vector3<TMathType> operator *(Vector3<TMathType> a, TMathType d)
		{
			return new Vector3<TMathType>(Operator.Multiply(a.x, d), Operator.Multiply(a.y, d), Operator.Multiply(a.z, d));
		}

		/// <summary>
		/// Scales the components of the vector by d.
		/// </summary>
		/// <param name="a">Vector to scale.</param>
		/// <param name="d">cale</param>
		public static Vector3<TMathType> operator *(TMathType d, Vector3<TMathType> a)
		{
			return new Vector3<TMathType>(Operator.Multiply(a.x, d), Operator.Multiply(a.y, d), Operator.Multiply(a.z, d));
		}

		/// <summary>
		/// Preforms component-wise vector subtraction.
		/// </summary>
		/// <param name="a">Vector one.</param>
		/// <param name="b">Vector two.</param>
		/// <returns></returns>
		public static Vector3<TMathType> operator -(Vector3<TMathType> a, Vector3<TMathType> b)
		{
			return new Vector3<TMathType>(Operator.Subtract(a.x, b.x), Operator.Subtract(a.y, b.y), Operator.Subtract(a.z, b.z));
		}

		/// <summary>
		/// Negates the vector's components.
		/// </summary>
		/// <param name="a">Vector to negate.</param>
		/// <returns></returns>
		public static Vector3<TMathType> operator -(Vector3<TMathType> a)
		{
			return new Vector3<TMathType>(Operator.Negate(a.x), Operator.Negate(a.y), Operator.Negate(a.z));
		}
		#endregion

		/// <summary>
		///   <para>Set x, y and z components of an existing Vector3<TMathType>.</para>
		/// </summary>
		/// <param name="new_x"></param>
		/// <param name="new_y"></param>
		/// <param name="new_z"></param>
		public void Set(TMathType new_x, TMathType new_y, TMathType new_z)
		{
			this.x = new_x;
			this.y = new_y;
			this.z = new_z;
		}
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			return sb.AppendFormat("{0}: {1}:{2}:{3}", nameof(Vector3<TMathType>), x, y, z).ToString();
		}

		public IEnumerator<TMathType> GetEnumerator()
		{
			//Not efficient way to do this
			return (new List<TMathType>(3) { x, y, z }).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			//Not efficient way to do this
			return (new TMathType[3] { x, y, z }).GetEnumerator();
		}

		//TODO: Implement these methods.
		/*/// <summary>
		///   <para>Moves a point current in a straight line towards a target point.</para>
		/// </summary>
		/// <param name="current"></param>
		/// <param name="target"></param>
		/// <param name="maxDistanceDelta"></param>
		public static Vector3<TMathType> MoveTowards(Vector3<TMathType> current, Vector3<TMathType> target, float maxDistanceDelta)
		{
			Vector3<TMathType> Vector3<TMathType> = target - current;
			float single = Vector3<TMathType>.magnitude;
			if (single <= maxDistanceDelta || single == 0f)
			{
				return target;
			}
			return current + ((Vector3 < TMathType > / single) * maxDistanceDelta);
		}
		/// <summary>
		///   <para>Returns the angle in degrees between from and to.</para>
		/// </summary>
		/// <param name="from">The angle extends round from this vector.</param>
		/// <param name="to">The angle extends round to this vector.</param>
		public static float Angle(Vector3<TMathType> from, Vector3<TMathType> to)
		{
			return Mathf.Acos(Mathf.Clamp(Vector3<TMathType>.Dot(from.normalized, to.normalized), -1f, 1f)) * 57.29578f;
		}*/
	}
}