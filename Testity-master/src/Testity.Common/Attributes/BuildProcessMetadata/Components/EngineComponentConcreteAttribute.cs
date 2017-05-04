using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Testity.Common
{
	/// <summary>
	/// Metadata marker for engine component concrete implementations.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class EngineComponentConcreteAttribute : Attribute
	{
		public readonly IEnumerable<Type> InterfaceComponentType;

		public EngineComponentConcreteAttribute(params Type[] interfaceTypes)
		{
			InterfaceComponentType = interfaceTypes;
		}
	}
}
