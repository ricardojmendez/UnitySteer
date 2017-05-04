using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testity.Common;
using Testity.EngineComponents.Unity3D;

namespace Testity.BuildProcess.Unity3D
{
	public class UnityComponentAdapterParser : IComponentAdapterParser
	{
		public class ComponentData : IComponentAdapterData
		{
			public EngineComponentAdapterConstructorAttribute AdapterConstructorMetadata { get; private set; }

			public EngineComponentAdapterAttribute AdapterMarkMetadata { get; private set; }

			public Type AdapterType { get; private set; }

			public ComponentData(EngineComponentAdapterAttribute adapterMetadata, EngineComponentAdapterConstructorAttribute constructorMetadata, Type adapterType)
			{
				AdapterType = adapterType;
				AdapterMarkMetadata = adapterMetadata;
				AdapterConstructorMetadata = constructorMetadata;
			}
		}

		public IEnumerable<IComponentAdapterData> ParseFor(Type typeToAdapt, Type targetTypeToAdaptTo)
		{
			//Lotta LINQ here but basically we need to build valid adapter types for the source -> dest types passed in.
			//This is done parsing the slew of metadata marked all over these
			 
			IEnumerable<Type> typesInAdapterAssembly = typeof(UnityEngineGameObjectAdapter).Assembly.GetTypes();

			//types that adapt to the target type
			IEnumerable<Type> componentAdapterType = typesInAdapterAssembly.Where(x => x.GetCustomAttribute<EngineComponentConcreteAttribute>(false) != null)
				.Where(x => x.GetCustomAttribute<EngineComponentConcreteAttribute>(false).InterfaceComponentType.Contains(targetTypeToAdaptTo));

			return componentAdapterType.Where(x => x.GetCustomAttribute<EngineComponentAdapterAttribute>(false) != null)
				.Where(x => x.GetCustomAttribute<EngineComponentAdapterAttribute>(false).ActualEngineType == typeToAdapt)
				.Where(x => x.GetConstructors().FirstOrDefault(y => y.GetCustomAttribute<EngineComponentAdapterConstructorAttribute>(false) != null && y.GetCustomAttribute<EngineComponentAdapterConstructorAttribute>(false).EngineTypeForConstruction == typeToAdapt) != null)
				.Select(x => new ComponentData(x.GetCustomAttribute<EngineComponentAdapterAttribute>(false), x.GetCustomAttribute<EngineComponentAdapterConstructorAttribute>(false), x));
		}
	}
}
