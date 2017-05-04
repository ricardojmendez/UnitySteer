using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testity.Common;

namespace Testity.BuildProcess
{
	public interface IComponentAdapterData
	{
		Type AdapterType { get; }

		EngineComponentAdapterAttribute AdapterMarkMetadata { get; }
		
		EngineComponentAdapterConstructorAttribute AdapterConstructorMetadata { get; }
	}
}
