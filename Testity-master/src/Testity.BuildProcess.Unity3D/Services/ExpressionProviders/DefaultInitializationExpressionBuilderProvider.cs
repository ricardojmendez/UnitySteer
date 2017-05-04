using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testity.BuildProcess.Unity3D
{
	public class DefaultInitializationExpressionBuilderProvider : IInitializationExpressionBuilderProvider
	{
		//Yep, this class looks super simple. Almost empty.
		//The reason for that is there are only two ways to handle initialization expressions
		//Either we find an adapter that is simple to generate a new instance of
		//otherwise for more complex things such as Vector3 to Vector3<int> we rely on some extension methods existing
		//to convert them.

		private class Builder : IInitializationExpressionBuilder
		{
			public IInitializationExpression Build(InitializationExpressionData data, string targetEngineComponentFieldName)
			{
				return new DefaultInitializationExpression(data, targetEngineComponentFieldName);
            }
		}

		public IInitializationExpressionBuilder FromReflectionData(Type sourceInfo, Type destInfo)
		{
			return new Builder();
		}
	}
}
