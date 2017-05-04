using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Testity.EngineComponents;

namespace Testity.EngineServices.Tests.UnitTests
{
	[TestFixture]
	public class EngineObjectReferenceDictionaryTests
	{
		[Test]
		public static void Test_ReferenceDictionary_Constructor()
		{
			//arrange
			Assert.DoesNotThrow(() => new EngineObjectReferenceDictionary<IEngineObject, object>());
		}

		[Test]
		public static void Test_ReferenceDictionary_Constructor_Size([Range(1, 20)] int size)
		{
			//arrange
			var map = new EngineObjectReferenceDictionary<IEngineObject, object>(size);

			//act
			FieldInfo info = map.GetType().BaseType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
				.FirstOrDefault(x => x.Name.Contains("bucket"));

			//assert
			//This will use reflection to get the collection size under the hood.
			Assert.AreEqual(GetPrime(size), ((int[])info.GetValue(map)).Count());
		}

		[Test]
		public static void Test_ReferenceDictionary_Register()
		{
			//arrange
			IEngineObject testEngineObject = SetupEquatableIsEqualToSelfInstance<IEngineObject>();
	object testObject = new object();
			var map = new EngineObjectReferenceDictionary<IEngineObject, object>();

			//act
			bool result = map.Register(testEngineObject, testObject);

			//assert
			Assert.IsTrue(result);
			Assert.IsTrue(map.ContainsKey(testEngineObject));
			Assert.AreEqual(testObject, map[testEngineObject]);

			//not in the dictionary so it should throw
			Assert.Throws<KeyNotFoundException>(() => { var o = map[Mock.Of<IEngineObject>()]; });
			Assert.IsFalse(map.ContainsKey(Mock.Of<IEngineObject>())); //shouldn't contain some new key value. Might fail due to Moq setup.
		}

		[Test]
		public static void Test_ReferenceDictionary_Unregister()
		{
			//arrange
			IEngineObject testEngineObject = SetupEquatableIsEqualToSelfInstance<IEngineObject>();
			object testObject = new object();
			var map = new EngineObjectReferenceDictionary<IEngineObject, object>();

			//act
			map.Register(testEngineObject, testObject);
			var result = map.TryUnregister(testEngineObject);
			var failedResult = map.TryUnregister(testEngineObject);

			//assert
			Assert.IsTrue(result.Success);
			Assert.AreEqual(result.Value, testObject);

			//check the intentional failed result
			Assert.IsFalse(failedResult.Success);
			Assert.IsNull(failedResult.Value);
		}

		[Test]
		public static void Test_ReferenceDictionary_TryLookup()
		{
			//arrange
			IEngineObject testEngineObject = SetupEquatableIsEqualToSelfInstance<IEngineObject>();
			object testObject = new object();
			var map = new EngineObjectReferenceDictionary<IEngineObject, object>();

			//act
			map.Register(testEngineObject, testObject);

			//assert
			Assert.AreEqual(testObject, map.TryLookup(testEngineObject));
			Assert.AreNotEqual(testObject, map.TryLookup(Mock.Of<IEngineObject>()));
			Assert.IsNull(map.TryLookup(Mock.Of<IEngineObject>()));

			Assert.Throws<ArgumentNullException>(() => map.TryLookup(null));
			Assert.Throws<ArgumentNullException>(() => map.Register(null, testObject));
			Assert.Throws<ArgumentNullException>(() => map.Register(testEngineObject, null));
			Assert.Throws<ArgumentNullException>(() => map.Register(null, null));
		}

		private static TEquatableInstance SetupEquatableIsEqualToSelfInstance<TEquatableInstance>()
			where TEquatableInstance : class, IEquatable<TEquatableInstance>
		{
			//arrange
			Mock<TEquatableInstance> testEngineObject = new Mock<TEquatableInstance>(MockBehavior.Strict);
			//setup IEquatable
			testEngineObject.Setup(x => x.Equals(testEngineObject.Object)).Returns(true);

			return testEngineObject.Object;
		}

		#region Source for HashHelpers
		//internal hashhelpers implementation from MS: http://www.dotnetframework.org/default.aspx/4@0/4@0/DEVDIV_TFS/Dev10/Releases/RTMRel/ndp/fx/src/Core/System/Collections/Generic/HashHelpers@cs/1305376/HashHelpers@cs
		private static int GetPrime(int min)
		{

			for (int i = 0; i < primes.Length; i++)
			{
				int prime = primes[i];
				if (prime >= min)
				{
					return prime;
				}
			}

			// Outside of our predefined table. Compute the hard way. 
			for (int i = (min | 1); i < Int32.MaxValue; i += 2)
			{
				if (IsPrime(i))
				{
					return i;
				}
			}
			return min;
		}

		private static readonly int[] primes = {
			3, 7, 11, 17, 23, 29, 37, 47, 59, 71, 89, 107, 131, 163, 197, 239, 293, 353, 431, 521, 631, 761, 919,
			1103, 1327, 1597, 1931, 2333, 2801, 3371, 4049, 4861, 5839, 7013, 8419, 10103, 12143, 14591,
			17519, 21023, 25229, 30293, 36353, 43627, 52361, 62851, 75431, 90523, 108631, 130363, 156437,
			187751, 225307, 270371, 324449, 389357, 467237, 560689, 672827, 807403, 968897, 1162687, 1395263,
			1674319, 2009191, 2411033, 2893249, 3471899, 4166287, 4999559, 5999471, 7199369};

		private static bool IsPrime(int candidate)
		{
			if ((candidate & 1) != 0)
			{
				int limit = (int)Math.Sqrt(candidate);
				for (int divisor = 3; divisor <= limit; divisor += 2)
				{
					if ((candidate % divisor) == 0)
					{
						return false;
					}
				}
				return true;
			}
			return (candidate == 2);
		}
		#endregion
	}
}
