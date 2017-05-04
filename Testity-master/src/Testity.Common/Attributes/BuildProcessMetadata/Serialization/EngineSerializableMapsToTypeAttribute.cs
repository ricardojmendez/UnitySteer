using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Testity.Common;

namespace Testity.Common
{
	[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Struct | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class EngineSerializableMapsToTypeAttribute : ImplementationMetadataAttribute
	{
		public readonly Type ConcreteEngineType;

		public EngineSerializableMapsToTypeAttribute(EngineType engineType, string typeName)
			: base(engineType)
		{
			ConcreteEngineType = Type.GetType(typeName, true, false);
		}
	}
}
