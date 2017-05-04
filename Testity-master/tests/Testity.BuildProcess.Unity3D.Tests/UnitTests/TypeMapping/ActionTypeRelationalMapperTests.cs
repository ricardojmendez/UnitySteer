using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testity.Unity3D.Events;
using UnityEngine.Events;

namespace Testity.BuildProcess.Unity3D
{
	[TestFixture]
	public static class ActionTypeRelationalMapperTests
	{
		[Test(Author = "Andrew Blakely", Description = "Tests to verify that the mapper can distinguish valid Action types.", TestOf = typeof(ActionTypeRelationalMapper))]
		[TestCase(typeof(Action))]
		[TestCase(typeof(Action<>))]
		[TestCase(typeof(Action<,>))]
		[TestCase(typeof(Action<,,>))]
		[TestCase(typeof(Action<,,,>))] //shouldn't go higher than 4 type args
		[TestCase(typeof(Action))]
		[TestCase(typeof(Action<int>))]
		[TestCase(typeof(Action<string,int>))]
		[TestCase(typeof(Action<object,int,string>))]
		[TestCase(typeof(Action<string,List<int>,string,object>))]
		public static void Test_ActionTypeMapper_isActionType_Returns_True_On_Valid_Action_Types(Type t)
		{
			//arrange
			ActionTypeRelationalMapper mapper = new ActionTypeRelationalMapper();

			//assert
			Assert.IsTrue(mapper.isActionType(t), "Expected type {0} to return true from {1}", t.ToString(), nameof(mapper.isActionType));
		}

		[Test(Author = "Andrew Blakely", Description = "Tests to verify that the mapper can distinguish invalid Action types.", TestOf = typeof(ActionTypeRelationalMapper))]
		[TestCase(typeof(string))]
		[TestCase(typeof(int))]
		[TestCase(typeof(Func<>))]
		[TestCase(typeof(object))]
		[TestCase(typeof(Action<,,,,>))]
		public static void Test_ActionTypeMapper_isActionType_Returns_False_On_Invalid_Types(Type t)
		{
			//arrange
			ActionTypeRelationalMapper mapper = new ActionTypeRelationalMapper();

			//assert
			Assert.IsFalse(mapper.isActionType(t), "Expected type {0} to return false from {1}", t.ToString(), nameof(mapper.isActionType));
		}

		[Test(Author = "Andrew Blakely", Description = "Tests to verify that the mapper maps to expected type.", TestOf = typeof(ActionTypeRelationalMapper))]
		[TestCase(typeof(Action), typeof(TestityEvent))]
		[TestCase(typeof(Action<int>), typeof(TestityEvent<int>))]
		[TestCase(typeof(Action<Action, int, string>), typeof(TestityEvent<Action, int, string>))]
		[TestCase(typeof(Action<string, object>), typeof(TestityEvent<string, object>))]
		public static void Test_ActionTypeMapper_Maps_To_Expected_UnityEvent_Type(Type t, Type expectedType)
		{
			//arrange
			ActionTypeRelationalMapper mapper = new ActionTypeRelationalMapper();

			//act
			Type resultType = mapper.ResolveMappedType(t);

			Assert.NotNull(resultType, "Failed to get non null type mapped for {0}", t);
			Assert.AreEqual(expectedType, resultType, "Failed to map {0} to {1}. Got {2} instead.", t, expectedType, resultType);
		}
	}
}
