using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Testity.Common.Attributes
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
	public class ImplementationRequiresParametersAttribute : ImplementationMetadataAttribute
	{
		public readonly IEnumerable<Type> ImplementationParameterTypes;

		public ImplementationRequiresParametersAttribute(EngineType type, params Type[] parameters)
			: base(type)
		{
			ImplementationParameterTypes = parameters;
		}

		public ImplementationRequiresParametersAttribute(EngineType type, params string[] parameters)
			: base(type)
		{
			ImplementationParameterTypes = parameters.Select(x => Type.GetType(x, true, false));
		}
	}
}
