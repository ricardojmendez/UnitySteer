using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Testity.Common;

namespace Testity.EngineComponents
{
	/// <summary>
	/// Implementer can be started by the engine. You may need this implemented on components that required initialization or configuring before the first update.
	/// Based on Unity3D's: http://docs.unity3d.com/ScriptReference/MonoBehaviour.Start.html
	/// </summary>
	[EngineEventInterface]
	public interface IEngineStartable
	{
		/// <summary>
		/// Called once before the first update of the engine after the implementer was created.
		/// </summary>
		[ImplementationRequiresName(EngineType.Unity3D, "Start")]
		void Start();
	}
}
