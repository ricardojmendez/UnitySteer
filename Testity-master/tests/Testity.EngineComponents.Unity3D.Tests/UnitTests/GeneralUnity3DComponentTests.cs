using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Testity.Common;

namespace Testity.EngineComponents.Unity3D.Tests
{
	[TestFixture]
	public static class GeneralUnity3DComponentTests
	{
		[Test(Author = "Andrew Blakely", Description = "Ensures all components are implemented by the target engine.")]
		public static void Ensure_All_Components_Are_Implemented()
		{
			//arrange
			//we need types that implement component interfaces
			IEnumerable<Type> componentTypes = typeof(IEngineObject).Assembly.GetTypes().Where(x => x.GetCustomAttribute<EngineComponentInterfaceAttribute>(false) != null);
			IEnumerable<Type> concreteComponentTypes = typeof(UnityEngineGameObjectAdapter).Assembly.GetTypes().Where(x => x.GetCustomAttribute<EngineComponentConcreteAttribute>(false) != null);
			
			//we make sure every engine component interface is implemented by sometime.
			foreach(Type interfaceType in componentTypes)
			{
				bool found = DoesTypeImplementInterface(concreteComponentTypes, interfaceType);

				Assert.IsTrue(found, "Unable to find class that implements: " + interfaceType);
			}
		}

		private static bool DoesTypeImplementInterface(IEnumerable<Type> toCheck, Type interfaceType)
		{
			//base case
			if (toCheck.Count() == 0)
				return false;

			foreach (Type t in toCheck)
			{
				foreach(Type interfaceT in t.GetInterfaces())
					if (interfaceType == interfaceT)
						return true;
			}

			//if we didn't find it recurr
			return DoesTypeImplementInterface(toCheck.Select(x => x.BaseType).Where(x => x != null), interfaceType);
		}
	}
}
