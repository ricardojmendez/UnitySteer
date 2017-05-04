using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testity.EngineComponents.Tests.UnitTests
{
	[TestFixture]
	public static class EngineScriptComponentTests
	{
		[Test(Author = "Andrew Blakely", Description = "Tests the base class constructor for " + nameof(EngineScriptComponent), TestOf = typeof(EngineScriptComponent))]
		public static void Test_Engine_Script_Component_Base_Constructor()
		{
			//Shouldn't throw or anything.
			EngineScriptComponent engineScriptComponent = Mock.Of<EngineScriptComponent>();
		}

		[Test(Author = "Andrew Blakely", Description = "Tests the base class constructor for " + nameof(EngineScriptComponent), TestOf = typeof(EngineScriptComponent))]
		public static void Test_Engine_Script_Component_Test_Equality()
		{
			//arrange
			Mock<EngineScriptComponent> engineScriptComponentMock = new Mock<EngineScriptComponent>(MockBehavior.Loose);
			Mock<EngineScriptComponent> engineScriptComponentSecondNonEqualMock = new Mock<EngineScriptComponent>(MockBehavior.Loose);

			engineScriptComponentMock.CallBase = true;
			engineScriptComponentSecondNonEqualMock.CallBase = true;

			EngineScriptComponent engineScriptComponent = engineScriptComponentMock.Object;
			EngineScriptComponent engineScriptComponentSecondNonEqual = engineScriptComponentSecondNonEqualMock.Object;

			//act (cast to interface for equal testing.
			IEngineObject engineObjectInterface = engineScriptComponent;
			IEngineObject engineObjectInterfaceTwo = engineScriptComponent;
			IEngineObject engineObjectInterfaceNotEqual = engineScriptComponentSecondNonEqual;

			//this tests equivalence. Not really Important because mostly you won't have references to the underlying script component and can't overload the equals.
			//assert
			Assert.AreEqual(engineScriptComponent, engineObjectInterface);
			Assert.IsTrue(engineScriptComponent == engineObjectInterface);
			Assert.IsTrue(engineObjectInterface == engineScriptComponent);
			Assert.IsTrue(engineObjectInterface.Equals(engineObjectInterfaceTwo));
			Assert.IsTrue(engineObjectInterface == engineObjectInterfaceTwo);
			
			Assert.IsTrue(engineScriptComponent.Equals(engineObjectInterface) == engineObjectInterface.Equals(engineScriptComponent));
			Assert.IsTrue(engineScriptComponent.Equals(engineObjectInterface));
			Assert.IsTrue(engineObjectInterface.Equals(engineScriptComponent));

			Assert.IsTrue(engineScriptComponentSecondNonEqual != engineScriptComponent);
			Assert.IsTrue(engineScriptComponent != engineScriptComponentSecondNonEqual);
			Assert.IsTrue(engineObjectInterface != engineObjectInterfaceNotEqual);

			Assert.IsTrue(engineScriptComponentSecondNonEqual != (IEngineObject)engineScriptComponent);
			Assert.IsTrue((IEngineObject)engineScriptComponentSecondNonEqual != engineScriptComponent);

			Assert.IsFalse(engineObjectInterface.Equals(engineScriptComponentSecondNonEqual));
			Assert.IsFalse(engineScriptComponent.Equals(engineScriptComponentSecondNonEqual));
		}
	}
}
