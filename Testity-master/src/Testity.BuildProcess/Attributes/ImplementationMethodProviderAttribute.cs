using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testity.BuildProcess.Attributes
{
	[AttributeUsage(AttributeTargets.Class)]
	public class ImplementationMethodProviderAttribute : Attribute
	{
		public readonly Type EngineEventType;

		public ImplementationMethodProviderAttribute(Type type)
		{
			EngineEventType = type;
		}
	}
}
