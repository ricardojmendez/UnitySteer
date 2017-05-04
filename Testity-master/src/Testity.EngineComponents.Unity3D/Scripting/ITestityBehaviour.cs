using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Testity.EngineComponents;

namespace Testity.EngineComponents.Unity3D
{
	public interface ITestityBehaviour<out TScriptComponentType> : ITestityBehaviour
		where TScriptComponentType : EngineScriptComponent
	{
		TScriptComponentType ScriptComponent { get; }
	}

	public interface ITestityBehaviour
	{
		void Initialize();

		object GetUntypedScriptComponent { get; }
	}
}
