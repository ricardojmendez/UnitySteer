using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Testity.Common
{
	public abstract class ImplementationMetadataAttribute : Attribute
	{
		public readonly EngineType EngineType;

		public ImplementationMetadataAttribute(EngineType type)
		{
			EngineType = type;
	}
	}
}
