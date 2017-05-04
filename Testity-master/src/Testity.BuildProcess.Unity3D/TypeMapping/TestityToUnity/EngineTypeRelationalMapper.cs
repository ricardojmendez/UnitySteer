using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fasterflect;
using Testity.Common;
using Testity.EngineComponents;

namespace Testity.BuildProcess.Unity3D
{
	public class EngineTypeRelationalMapper : ITypeRelationalMapper
	{
		public Type ResolveMappedType(Type typeToFindRelation)
		{
			if (typeToFindRelation == null)
				throw new ArgumentNullException(nameof(typeToFindRelation), "Type cannot be null.");

			//We need to find the [EngineSerializableMapsToType] attribute that is for UnityEngine
			EngineSerializableMapsToTypeAttribute mapInfo = typeToFindRelation.Attributes<EngineSerializableMapsToTypeAttribute>()
				.FirstOrDefault(x => x.EngineType == EngineType.Unity3D);

			//If we found the attribute we return the mapped type and if not then null
			return mapInfo?.ConcreteEngineType;
        }
	}
}
