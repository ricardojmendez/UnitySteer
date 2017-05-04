using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testity.BuildProcess.Unity3D
{
	/// <summary>
	/// Attempts to map a Type to a primitive Type.
	/// </summary>
	public class PrimitiveTypeRelationalMapper : ITypeRelationalMapper
	{
		private readonly IEnumerable<Type> excludedPrimitiveTypes;

		public PrimitiveTypeRelationalMapper(IEnumerable<Type> typesToExclude)
		{
			excludedPrimitiveTypes = typesToExclude;
        }

		public Type ResolveMappedType(Type typeToFindRelation)
		{
			if (typeToFindRelation == null)
				throw new ArgumentNullException(nameof(typeToFindRelation), "Type cannot be null.");

			//if it's a primitive we just return the type
			if (typeToFindRelation.IsPrimitive)
			{
				//make sure the type is not excluded
				if (excludedPrimitiveTypes.Contains(typeToFindRelation))
					throw new InvalidOperationException("Cannot serialize primitive Type: " + typeToFindRelation.ToString());

				return typeToFindRelation;
			}
			else
				return null;
		}
	}
}
