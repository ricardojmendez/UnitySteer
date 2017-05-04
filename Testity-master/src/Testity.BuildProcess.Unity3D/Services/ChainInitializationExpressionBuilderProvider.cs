using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Testity.BuildProcess.Unity3D
{
	public class ChainInitializationExpressionBuilderProvider : IInitializationExpressionBuilderProvider
	{
		private IEnumerable<IInitializationExpressionBuilderProvider> builderProviders;

		public ChainInitializationExpressionBuilderProvider(IEnumerable<IInitializationExpressionBuilderProvider> providers)
		{
			builderProviders = providers;
        }

		public IInitializationExpressionBuilder FromReflectionData(Type sourceInfo, Type destInfo)
		{
			foreach(IInitializationExpressionBuilderProvider p in builderProviders)
			{
				IInitializationExpressionBuilder builder = p.FromReflectionData(sourceInfo, destInfo);

				if (builder != null)
					return builder;
			}

			return null;
		}
	}
}
