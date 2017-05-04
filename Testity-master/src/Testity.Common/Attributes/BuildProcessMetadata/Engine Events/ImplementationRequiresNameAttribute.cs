using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Testity.Common
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
	public class ImplementationRequiresNameAttribute : ImplementationMetadataAttribute
	{
		public readonly string ImplementationName;

		public ImplementationRequiresNameAttribute(EngineType type, string name)
			: base(type)
		{
			ImplementationName  = name;
	}
	}
}
