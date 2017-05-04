using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testity.EngineComponents;
using Testity.EngineComponents.Unity3D;

namespace Testity.BuildProcess.Unity3D
{
	public class AddTestityBehaviourBaseClassMemberStep : ITestityBuildStep
	{
		public void Process(IClassBuilder builder, Type typeToParse)
		{
			if (builder == null)
				throw new ArgumentNullException(nameof(builder), "Class builder cannot be null.");

			if (typeToParse == null)
				throw new ArgumentNullException(nameof(typeToParse), "Type for the generic arg cannot be null.");

#if DEBUG || DEBUGBUILD
			if (!typeof(EngineScriptComponent).IsAssignableFrom(typeToParse))
				throw new InvalidOperationException("Cannot parse Type: " + typeToParse + " for a " + typeof(TestityBehaviour<>) + " because it doesn't inherit from " + typeof(EngineScriptComponent));
#endif

			builder.AddBaseClass(new DefaultTypeSyntaxBuilder(), typeof(TestityBehaviour<>).MakeGenericType(typeToParse));
		}
	}
}
