using MiscUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Testity.Common;

namespace Testity.EngineMath
{
	//For information on how and why this is efficient see: http://stackoverflow.com/questions/1348594/is-there-a-c-sharp-generic-constraint-for-real-number-types
	//A library by the legendary Jon Skeet and Marc Gravell cannot be dismissed. It works.
	/// <summary>
	/// Generic vector type with 2 components. Allows for use of int, float and double types.
	/// Based on Unity3D's: http://docs.unity3d.com/ScriptReference/Vector2.html
	/// </summary>
	/// <typeparam name="TMathType">Value type of the vector that must overload math operators (Ex. int, float, double).</typeparam>
	[EngineSerializableMapsToType(EngineType.Unity3D, "UnityEngine.Vector2, UnityEngine")]
	public struct Vector2<TMathType> : IEquatable<Vector2<TMathType>>
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
		/// A cache of the <see cref="TMathType"/> value that represents 1.
		/// </summary>
		private static TMathType oneValue = MathT.GenerateOneValue<TMathType>();

		/// <summary>
		///   <para>Shorthand for writing Vector2<TMathType>(0, -1, 0).</para>
		/// </summary>
		public static Vector2<TMathType> down
		{
			get
			{
				return new Vector2<TMathType>(Operator<TMathType>.Zero, Operator.Subtract(Operator<TMathType>.Zero, Vector2<TMathType>.oneValue));
			}
		}

		/// <summary>
		///   <para>Shorthand for writing Vector2<TMathType>(-1, 0, 0).</para>
		/// </summary>
		public static Vector2<TMathType> left
		{
			get
			{
				return new Vector2<TMathType>(Operator.Subtract(Operator<TMathType>.Zero, Vector2<TMathType>.oneValue), Operator<TMathType>.Zero);
			}
		}

		/// <summary>
		///   <para>Shorthand for writing Vector2<TMathType>(1, 1, 1).</para>
		/// </summary>
		public static Vector2<TMathType> one
		{
			get
			{
				return new Vector2<TMathType>(Vector2<TMathType>.oneValue, Vector2<TMathType>.oneValue);
			}
		}

		/// <summary>
		///   <para>Shorthand for writing Vector2<TMathType>(1, 0, 0).</para>
		/// </summary>
		public static Vector2<TMathType> right
		{
			get
			{
				return new Vector2<TMathType>(Vector2<TMathType>.oneValue, Operator<TMathType>.Zero);

			}
		}

		/// <summary>
		///   <para>Shorthand for writing Vector2<TMathType>(0, 1, 0).</para>
		/// </summary>
		public static Vector2<TMathType> up
		{
			get
			{
				return new Vector2<TMathType>(Operator<TMathType>.Zero, Vector2<TMathType>.oneValue);
			}
		}

		/// <summary>
		///   <para>Shorthand for writing Vector2<TMathType>(0, 0, 0).</para>
		/// </summary>
		public static Vector2<TMathType> zero
		{
			get
			{
				return new Vector2<TMathType>(Operator<TMathType>.Zero, Operator<TMathType>.Zero);
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
				}
				throw new IndexOutOfRangeException("Invalid " + nameof(Vector2<TMathType>) + " index!");
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
					default:
						throw new IndexOutOfRangeException("Invalid " + nameof(Vector2<TMathType>) + " index!");
				}
			}
		}

		/// <summary>
		///   <para>Creates a new vector with given x and y components.</para>
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public Vector2(TMathType x, TMathType y)
		{
			this.x = x;
			this.y = y;
		}

		/// <summary>
		///   <para>Creates a new vector with given x and sets y to zero.</para>
		/// </summary>
		/// <param name="x"></param>
		public Vector2(TMathType x)
			: this(x, Operator<TMathType>.Zero)
		{

		}

		public override bool Equals(object other)
		{
			if (!(other is Vector2<TMathType>))
			{
				return false;
			}

			Vector2<TMathType> Vector2 = (Vector2<TMathType>)other;
			return this.Equals(Vector2); //calls generic version
		}

		public bool Equals(Vector2<TMathType> other)
		{
			return this.x.Equals(other.x) && this.y.Equals(other.y);
		}

		//No idea what is going on here... This is Unity3D's decompiled GetHashCode implementation.
		public override int GetHashCode()
		{
			return this.x.GetHashCode() ^ this.y.GetHashCode();
		}

		/// <summary>
		///   <para>Returns a vector that is made from the largest components of two vectors.</para>
		/// </summary>
		/// <param name="lhs"></param>
		/// <param name="rhs"></param>
		public static Vector2<TMathType> Max(Vector2<TMathType> lhs, Vector2<TMathType> rhs)
		{
			return new Vector2<TMathType>(MathT.Max(lhs.x, rhs.x), MathT.Max(lhs.y, rhs.y));
		}

		/// <summary>
		///   <para>Returns a vector that is made from the smallest components of two vectors.</para>
		/// </summary>
		/// <param name="lhs"></param>
		/// <param name="rhs"></param>
		public static Vector2<TMathType> Min(Vector2<TMathType> lhs, Vector2<TMathType> rhs)
		{
			return new Vector2<TMathType>(MathT.Min(lhs.x, rhs.x), MathT.Min(lhs.y, rhs.y));
		}

		#region Operator Overloads
		public static Vector2<TMathType> operator +(Vector2<TMathType> a, Vector2<TMathType> b)
		{
			return new Vector2<TMathType>(Operator.Add(a.x, b.x), Operator.Add(a.y, b.y));
		}

		public static bool operator ==(Vector2<TMathType> lhs, Vector2<TMathType> rhs)
		{
			//Do componentwise comaparision instead since it's more reliable
			for (int i = 0; i < 2; i++)
				if (!lhs[i].Equals(rhs[i]))
					return false;

			return true;
			//Don't do it like this. We can't be 100% sure it will work for all types.
			//return Operator.LessThan(Vector2<TMathType>.SqrMagnitude<TMathType>(lhs - rhs), Vector2<TMathType>.validCompareError);
		}

		public static bool operator !=(Vector2<TMathType> lhs, Vector2<TMathType> rhs)
		{
			return !(lhs == rhs);
			//Don't do it like this. We can't be 100% sure it will work for all types.
			//return Operator.GreaterThanOrEqual(Vector2<TMathType>.SqrMagnitude(lhs - rhs), Vector2<TMathType>.validCompareError);
		}

		/// <summary>
		/// Scales the components of the vector by d.
		/// </summary>
		/// <param name="a">Vector to scale.</param>
		/// <param name="d">cale</param>
		/// <returns></returns>
		public static Vector2<TMathType> operator *(Vector2<TMathType> a, TMathType d)
		{
			return new Vector2<TMathType>(Operator.Multiply(a.x, d), Operator.Multiply(a.y, d));
		}

		/// <summary>
		/// Scales the components of the vector by d.
		/// </summary>
		/// <param name="a">Vector to scale.</param>
		/// <param name="d">cale</param>
		public static Vector2<TMathType> operator *(TMathType d, Vector2<TMathType> a)
		{
			return new Vector2<TMathType>(Operator.Multiply(a.x, d), Operator.Multiply(a.y, d));
		}

		/// <summary>
		/// Preforms component-wise vector subtraction.
		/// </summary>
		/// <param name="a">Vector one.</param>
		/// <param name="b">Vector two.</param>
		/// <returns></returns>
		public static Vector2<TMathType> operator -(Vector2<TMathType> a, Vector2<TMathType> b)
		{
			return new Vector2<TMathType>(Operator.Subtract(a.x, b.x), Operator.Subtract(a.y, b.y));
		}

		/// <summary>
		/// Negates the vector's components.
		/// </summary>
		/// <param name="a">Vector to negate.</param>
		/// <returns></returns>
		public static Vector2<TMathType> operator -(Vector2<TMathType> a)
		{
			return new Vector2<TMathType>(Operator.Negate(a.x), Operator.Negate(a.y));
		}
		#endregion

		/// <summary>
		///   <para>Set x and y components of an existing Vector2<TMathType>.</para>
		/// </summary>
		/// <param name="new_x"></param>
		/// <param name="new_y"></param>
		/// <param name="new_z"></param>
		public void Set(TMathType new_x, TMathType new_y)
		{
			this.x = new_x;
			this.y = new_y;
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			return sb.AppendFormat("{0}: {1}:{2}", nameof(Vector2<TMathType>), x, y).ToString();
		}
	}
}
