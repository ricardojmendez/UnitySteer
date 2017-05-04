using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Testity.Common
{
	/// <summary>
	/// Metadata that marks the constructor for adapter creation.
	/// </summary>
	[AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
	public class EngineComponentAdapterConstructorAttribute : Attribute
	{
		public readonly Type EngineTypeForConstruction;

		public EngineComponentAdapterConstructorAttribute(Type engineType)
		{
			EngineTypeForConstruction = engineType;
		}
	}
}
