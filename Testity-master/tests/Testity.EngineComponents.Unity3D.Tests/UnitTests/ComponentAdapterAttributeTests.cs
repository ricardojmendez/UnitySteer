using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Testity.Common;

namespace Testity.EngineComponents.Unity3D.Tests
{
	[TestFixture]
	public static class ComponentAdapterTests
	{
		[Test(Author = "Andrew Blakely", Description = "Verifies that component adapters have valid Unity3D Types.", TestOf = typeof(EngineComponentAdapterAttribute))]
		[TestCaseSource(nameof(adapterTypes))]
		public static void Verify_Component_Adapters_Contain_Valid_Unity3D_Types(Type adapterType)
		{
			//arrange
			EngineComponentAdapterAttribute adapterAttri = adapterType.GetCustomAttribute<EngineComponentAdapterAttribute>(false);

			//assert
			Assert.IsTrue(adapterAttri.ActualEngineType.Assembly == typeof(UnityEngine.Object).Assembly, "Type {0} doesn't seem to be a UnityEngine type adapter. Check {1} attribute.",
					adapterType.FullName, typeof(EngineComponentAdapterAttribute).FullName);
		}

		[Test(Author = "Andrew Blakely", Description = "Verifies that component adapters have metadata marked valid constructors.", TestOf = typeof(EngineComponentAdapterAttribute))]
		[TestCaseSource(nameof(adapterTypes))]
		public static void Verify_Component_Adapters_Marked_With_Valid_Adapter_Constructors(Type adapterType)
		{
			//arrange
			EngineComponentAdapterConstructorAttribute constructorAttri = adapterType.GetConstructors(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
				.Select(x => x.GetCustomAttribute<EngineComponentAdapterConstructorAttribute>(false))
				.Where(x => x != null)
				.FirstOrDefault();

			EngineComponentAdapterAttribute adapterAttri = adapterType.GetCustomAttribute<EngineComponentAdapterAttribute>(false);

			//assert
			Assert.NotNull(constructorAttri, "Failed to find constructor metadata marker for: {0}", adapterType.ToString());
			Assert.AreEqual(adapterAttri.ActualEngineType, constructorAttri.EngineTypeForConstruction, "Expected same types for metadata pair.");
		}

		private static Type[] adapterTypes = typeof(Testity.EngineComponents.Unity3D.UnityEngineGameObjectAdapter)
				.Assembly.GetTypes().Where(x => x.GetCustomAttribute<EngineComponentAdapterAttribute>(false) != null).ToArray();
    }
}
