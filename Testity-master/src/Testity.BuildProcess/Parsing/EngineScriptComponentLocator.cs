using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Fasterflect;
using Testity.EngineComponents;

namespace Testity.BuildProcess
{
	public class EngineScriptComponentLocator
	{
		private readonly Assembly assemblyToParse;

		public EngineScriptComponentLocator(Assembly assembly)
		{
			assemblyToParse = assembly;
		}

		public IEnumerable<Type> LoadTypes()
		{
			//this should be cached behind the scenes by fasterflect.
			//We do not want abstract classes though
			return assemblyToParse.TypesImplementing<EngineScriptComponent>().Where(x => !x.IsAbstract);
		}
	}
}
