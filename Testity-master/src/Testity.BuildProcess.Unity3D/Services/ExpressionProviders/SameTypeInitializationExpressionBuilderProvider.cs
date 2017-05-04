using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testity.BuildProcess.Unity3D
{
	public class SameTypeInitializationExpressionBuilderProvider : IInitializationExpressionBuilderProvider
	{
		private class Builder : IInitializationExpressionBuilder
		{
			public IInitializationExpression Build(InitializationExpressionData data, string targetEngineComponentFieldName)
			{
				return new SameTypeInitialization(data, targetEngineComponentFieldName);
            }
		}

		public IInitializationExpressionBuilder FromReflectionData(Type sourceInfo, Type destInfo)
		{
			//if they're the same type then we should be able to just assign them
			if (sourceInfo == destInfo)
				return new Builder();

			//Maybe it's a type that is assignment from the other type
			//If so then we can also just assign it. In this can maybe we want an IEnumerable<T> but it's a List<T>. Not that that would happen though.
			if (destInfo.IsAssignableFrom(sourceInfo))
				return new Builder();

			//we can't handle it if it didn't meet the above criteria
			return null;
		}
	}
}
