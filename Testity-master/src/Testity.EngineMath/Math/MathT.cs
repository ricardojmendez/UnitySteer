using MiscUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Testity.EngineMath
{
	/// <summary>
	/// A static generic math functionality class for computing.
	/// Based on Unity3D's: http://docs.unity3d.com/ScriptReference/Mathf.html
	/// </summary>
	public static class MathT
	{
		internal static TMathType GenerateKEpslion<TMathType>()
		{
			try
			{
				return (TMathType)Convert.ChangeType(1E-05f, typeof(TMathType));
			}
			catch (InvalidCastException)
			{
#if DEBUG || DEBBUGBUILD
				//These are known types that cause issues with kEpsilon
				if (typeof(TMathType) != typeof(char))
					throw;
#endif
				return Operator<TMathType>.Zero;
			}
		}

		internal static TMathType GenerateOneValue<TMathType>()
		{
			return (TMathType)Convert.ChangeType(1, typeof(TMathType));
		}

		internal static TMathType GenerateTwoValue<TMathType>()
		{
			return (TMathType)Convert.ChangeType(2, typeof(TMathType));
		}

		public static TMathType Sqrt<TMathType>(TMathType val)
			where TMathType : struct
		{
			return (TMathType)Convert.ChangeType(Math.Sqrt((double)Convert.ChangeType(val, typeof(double))), typeof(TMathType));
		}

		public static TMathType Acos<TMathType>(TMathType val)
		{
			return (TMathType)Convert.ChangeType(Math.Acos((double)Convert.ChangeType(val, typeof(double))), typeof(TMathType));
		}

		public static TMathType Max<TMathType>(TMathType val1, TMathType val2)
			where TMathType : struct
		{
			return Operator<TMathType>.GreaterThanOrEqual(val1, val2) ? val1 : val2;
		}

		public static TMathType Min<TMathType>(TMathType val1, TMathType val2)
			where TMathType : struct
		{
			return Operator<TMathType>.LessThanOrEqual(val1, val2) ? val1 : val2;
		}
	}

	public static class MathT<TMathType>
		where TMathType : struct
	{
		private static Dictionary<Type, Func<TMathType, TMathType>> absMathFuncDict;

		static MathT()
		{
			absMathFuncDict = new Dictionary<Type, Func<TMathType, TMathType>>(5);
		}

		public static TMathType Abs(TMathType val)
		{
			if (!absMathFuncDict.ContainsKey(typeof(TMathType)))
				absMathFuncDict[typeof(TMathType)] = (Func<TMathType, TMathType>)Delegate.CreateDelegate(typeof(Func<TMathType, TMathType>), typeof(Math).GetMethod("Abs", new Type[] { typeof(TMathType) }, null));

			return absMathFuncDict[typeof(TMathType)](val);
		}
	}
}
