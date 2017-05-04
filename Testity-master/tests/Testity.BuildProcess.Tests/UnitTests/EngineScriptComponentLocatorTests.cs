using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Testity.EngineComponents;

namespace Testity.BuildProcess.Tests
{
	[TestFixture]
	public static class EngineScriptComponentLocatorTests
	{
		[Test(Author = "Andrew Blakely", Description = "Tests the Rosyln compilation addition of using statements TestityClassBuilder.", TestOf = typeof(TestityClassBuilder))]
		[TestCase(typeof(TestClass1), true)]
		[TestCase(typeof(TestClass2), true)]
		[TestCase(typeof(TestClass4), true)] //down the hierarchy should be found
		[TestCase(typeof(EngineScriptComponentLocatorTests), false)] //non engine component should not be found
		[TestCase(typeof(TestClass3), false)] //abstract should not be found
		[TestCase(typeof(TestClass5), true)] //classes that inherit from abstracts should be found
		public static void Test_EngineScriptComponentLocator_Load_Types(Type typeToTest, bool shouldContain)
		{
			//arrange
			EngineScriptComponentLocator locator = new EngineScriptComponentLocator(Assembly.GetExecutingAssembly());

			//act
			IEnumerable<Type> types = locator.LoadTypes().ToList();

			//assert
			Assert.AreEqual(shouldContain, types.Contains(typeToTest), "Expected types to contain Type: {0}", typeToTest.ToString());
		}

		public class TestClass1 : EngineScriptComponent
		{

		}

		public class TestClass2 : EngineScriptComponent
		{

		}

		public abstract class TestClass3 : EngineScriptComponent
		{

		}

		public class TestClass4 : TestClass2
		{

		}

		public class TestClass5 : TestClass3
		{

		}
	}
}
