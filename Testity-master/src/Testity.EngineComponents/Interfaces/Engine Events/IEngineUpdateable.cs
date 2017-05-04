using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Testity.Common;

namespace Testity.EngineComponents
{
	/// <summary>
	/// Implementer can be updated by the engine.
	/// Based on Unity3D's: http://docs.unity3d.com/ScriptReference/MonoBehaviour.Update.html
	/// </summary>
	[EngineEventInterface]
	public interface IEngineUpdateable
	{
		/// <summary>
		/// Called everytime the engine updates in an unfixed timestep.
		/// </summary>
		[ImplementationRequiresName(EngineType.Unity3D, "Update")]
		void Update();
	}
}
