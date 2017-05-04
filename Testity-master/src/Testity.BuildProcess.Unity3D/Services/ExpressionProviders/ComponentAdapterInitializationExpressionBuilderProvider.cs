using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Fasterflect;

namespace Testity.BuildProcess.Unity3D
{
	public class ComponentAdapterInitializationExpressionBuilderProvider : IInitializationExpressionBuilderProvider
	{
		private readonly IComponentAdapterParser adapterParser;

		private class Builder : IInitializationExpressionBuilder
		{
			private readonly Type adapterType;

			public Builder(Type type)
			{
				adapterType = type;
			}

			public IInitializationExpression Build(InitializationExpressionData data, string targetEngineComponentFieldName)
			{
				return new ComponentAdapterInitializationExpression(data, adapterType, targetEngineComponentFieldName);
			}
		}

		public ComponentAdapterInitializationExpressionBuilderProvider(IComponentAdapterParser parser)
		{
			adapterParser = parser;
		}

		public IInitializationExpressionBuilder FromReflectionData(Type sourceInfo, Type destInfo)
		{
			//try to find an adapter for it
			IEnumerable<IComponentAdapterData> datas = adapterParser.ParseFor(sourceInfo, destInfo);

			//if there is an adapter then we can adapt this type with it
			if (datas.Count() != 0)
				return new Builder(datas.First().AdapterType);
			else
				return null;
        }
	}
}
