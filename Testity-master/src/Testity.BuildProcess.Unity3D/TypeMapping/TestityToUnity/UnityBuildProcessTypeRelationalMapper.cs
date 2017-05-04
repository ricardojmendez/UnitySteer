using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testity.BuildProcess.Unity3D
{
	public class UnityBuildProcessTypeRelationalMapper : ITypeRelationalMapper
	{
		private readonly IEnumerable<ITypeRelationalMapper> typeRelationalMapperChain;

		public UnityBuildProcessTypeRelationalMapper(IEnumerable<ITypeRelationalMapper> mapperChain)
		{
			if(mapperChain == null)
				throw new ArgumentNullException(nameof(mapperChain), "The mapper collection must not be null.");

			if (mapperChain.Count() == 0)
				throw new ArgumentException("Mapper chain must have at least one mapper.", nameof(mapperChain));

			typeRelationalMapperChain = mapperChain;
        }

		public Type ResolveMappedType(Type typeToFindRelation)
		{
			if (typeToFindRelation == null)
				throw new ArgumentNullException(nameof(typeToFindRelation), "Type cannot be null.");

			foreach (ITypeRelationalMapper m in typeRelationalMapperChain)
			{
				Type resultType = m.ResolveMappedType(typeToFindRelation);

				if (resultType != null)
					return resultType;
			}

			throw new ArgumentException("Unable to find mapped type for: " + typeToFindRelation.ToString(), nameof(typeToFindRelation));
		}
	}
}
