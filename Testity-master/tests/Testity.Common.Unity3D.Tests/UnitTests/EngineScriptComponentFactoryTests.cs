using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Testity.EngineComponents;

namespace Testity.Common.Unity3D.Tests
{
	[TestFixture]
	public class EngineScriptComponentFactoryTests
	{
		public class TestEngineScriptBase : EngineScriptComponent
		{
			[ExposeDataMemeber]
			private float privateFloatField;

			[ExposeDataMemeber]
			protected readonly int protectedReadonlyIntField;
		}

		public class TestEngineScriptChild : TestEngineScriptBase
		{
			[ExposeDataMemeber]
			private string privateStringProperty { get; set; }
		}

		[Test]
		public static void Test_EngineScriptComponentFactoryCreate_EngineScriptComponent()
		{
			//arrange
			EngineScriptComponent script = EngineScriptComponentFactory.Create<TestEngineScriptChild>();
			EngineScriptComponent scriptBase = EngineScriptComponentFactory.Create<TestEngineScriptBase>();

			//assert
			//first
			Assert.NotNull(script);
			Assert.IsInstanceOf<TestEngineScriptChild>(script);
			Assert.IsInstanceOf<TestEngineScriptBase>(script);

			//We have to make sure it gives us a proper one even if we've created and cached a similar one before
			//second
			Assert.NotNull(scriptBase);
			Assert.IsInstanceOf<TestEngineScriptBase>(scriptBase);
			Assert.IsNotInstanceOf<TestEngineScriptChild>(scriptBase);
		}
	}
}
