using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testity.BuildProcess.Unity3D
{
	public class StringTypeRelationalMapper : ITypeRelationalMapper
	{
		public Type ResolveMappedType(Type typeToFindRelation)
		{
			//String isn't a primitive so it won't be handled in the primitive handler
			//This means we require a special unique handler for string.

			if (typeToFindRelation == typeof(string))
				return typeof(string);
			else
				return null;
		}
	}
}
