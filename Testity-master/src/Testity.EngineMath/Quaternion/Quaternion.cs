using MiscUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Testity.EngineMath
{
	public struct Quaternion<TMathType>
		where TMathType : struct, IEquatable<TMathType>, IComparable<TMathType>
	{
		public static TMathType kEpsilon = MathT.GenerateKEpslion<TMathType>();

		/// <summary>
		/// A cache of the <see cref="TMathType"/> value that represents 1.
		/// </summary>
		private static TMathType oneValue = MathT.GenerateOneValue<TMathType>();

		private static TMathType twoValue = MathT.GenerateTwoValue<TMathType>();

		private static TMathType dotCompareVal = GenerateDotCompareValue();
		private static TMathType GenerateDotCompareValue()
		{
			try
			{
				return (TMathType)Convert.ChangeType(0.999999f, typeof(TMathType));
			}
			catch (InvalidCastException)
			{
#if DEBUG || DEBBUGBUILD
				//These are known types that cause issues with quat
				if (typeof(TMathType) != typeof(char))
					throw;
				//These are known types that cause issues with quat
				if (typeof(TMathType) != typeof(byte))
					throw;
				//These are known types that cause issues with quat
				if (typeof(TMathType) != typeof(int))
					throw;
#endif
				return Operator<TMathType>.Zero;
			}
		}

		//This let's us compute dot products with strong typing in another method external from the class.
		//WARNING: JIT/CLR or whatever will not produce the same result compared to directly calling the extension method.
		private delegate TMathType LowGCDotProductFunc(ref Quaternion<TMathType> a, ref Quaternion<TMathType> b);
		private static LowGCDotProductFunc dotFunc;

		/// <summary>
		///   <para>X component of the Quaternion<TMathType>. Don't modify this directly unless you know Quaternion<TMathType>s inside out.</para>
		/// </summary>
		public TMathType x;

		/// <summary>
		///   <para>Y component of the Quaternion<TMathType>. Don't modify this directly unless you know Quaternion<TMathType>s inside out.</para>
		/// </summary>
		public TMathType y;

		/// <summary>
		///   <para>Z component of the Quaternion<TMathType>. Don't modify this directly unless you know Quaternion<TMathType>s inside out.</para>
		/// </summary>
		public TMathType z;

		/// <summary>
		///   <para>W component of the Quaternion<TMathType>. Don't modify this directly unless you know Quaternion<TMathType>s inside out.</para>
		/// </summary>
		public TMathType w;

		/// <summary>
		///   <para>The identity rotation (Read Only).</para>
		/// </summary>
		public static Quaternion<TMathType> identity { get { return new Quaternion<TMathType>(Operator<TMathType>.Zero, Operator<TMathType>.Zero, Operator<TMathType>.Zero, oneValue); } }


		/// <summary>
		/// Gets and Sets a <see cref="Vector3{TMathType}"/> of the xyz components.
		/// Part of decompiled Unity3D Quat: https://gist.github.com/HelloKitty/91b7af87aac6796c3da9
		/// </summary>
		public Vector3<TMathType> xyz
		{
			set
			{
				x = value.x;
				y = value.y;
				z = value.z;
			}
			get
			{
				return new Vector3<TMathType>(x, y, z);
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
					case 3:
							return this.w;
					default:
						throw new IndexOutOfRangeException("Invalid " + nameof(Quaternion<TMathType>) + " index!");
				}
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
					case 3:
						this.w = value;
						break;
					default:
						throw new IndexOutOfRangeException("Invalid " + nameof(Quaternion<TMathType>) + " index!");
				}
			}
		}

		static Quaternion()
		{
			//We need to create a delegate that points to the dot product ext
			//TODO: Remove method search
			MethodInfo dotMethodInfo = typeof(QuaternionExtensions)
				.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
				.First(x => x.Name == "DotRef" && x.ReturnType == typeof(TMathType));

			dotFunc = (LowGCDotProductFunc)Delegate.CreateDelegate(typeof(LowGCDotProductFunc), dotMethodInfo, true);
		}

		//Additional constructors added to easily support implementation found here: https://gist.github.com/HelloKitty/91b7af87aac6796c3da9

		/// <summary>
		///   <para>Constructs new Quaternion with given x,y,z,w components.</para>
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		/// <param name="w"></param>
		public Quaternion(TMathType x, TMathType y, TMathType z, TMathType w)
		{
			this.x = x;
			this.y = y;
			this.z = z;
			this.w = w;
		}

		/// <summary>
		/// Construct a new Quaternion from vector and w components
		/// </summary>
		/// <param name="v">The vector part</param>
		/// <param name="w">The w part</param>
		public Quaternion(Vector3<TMathType> v, TMathType w)
		{
			this.x = v.x;
			this.y = v.y;
			this.z = v.z;
			this.w = w;
		}


		/// <summary>
		///   <para>Set x, y, z and w components of an existing Quaternion.</para>
		/// </summary>
		/// <param name="new_x"></param>
		/// <param name="new_y"></param>
		/// <param name="new_z"></param>
		/// <param name="new_w"></param>
		public void Set(TMathType new_x, TMathType new_y, TMathType new_z, TMathType new_w)
		{
			this.x = new_x;
			this.y = new_y;
			this.z = new_z;
			this.w = new_w;
		}

		public override bool Equals(object other)
		{
			if (!(other is Quaternion<TMathType>))
			{
				return false;
			}

			Quaternion<TMathType> quat = (Quaternion<TMathType>)other;
			return (!this.x.Equals(quat.x) || !this.y.Equals(quat.y) || !this.z.Equals(quat.z) ? false : this.w.Equals(quat.w));
		}

		public override int GetHashCode()
		{
			return this.x.GetHashCode() ^ this.y.GetHashCode() << 2 ^ this.z.GetHashCode() >> 2 ^ this.w.GetHashCode() >> 1;
		}

		public static bool operator ==(Quaternion<TMathType> lhs, Quaternion<TMathType> rhs)
		{
			return Operator.GreaterThan(dotFunc(ref lhs, ref rhs), dotCompareVal);
		}

		public static bool operator !=(Quaternion<TMathType> lhs, Quaternion<TMathType> rhs)
		{
			return Operator.LessThanOrEqual(dotFunc(ref lhs, ref rhs), dotCompareVal);
		}
	}
}
