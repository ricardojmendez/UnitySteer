using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Testity.Common;

namespace Testity.EngineComponents
{
	/// <summary>
	/// Implementer can be updated by the engine in a fixed time step.
	/// Based on Unity3D's: http://docs.unity3d.com/ScriptReference/MonoBehaviour.FixedUpdate.html
	/// </summary>
	[EngineEventInterface]
	public interface IEngineFixedUpdateable
	{
		/// <summary>
		/// Called everytime the engine updates in a fixed timestep.
		/// </summary>
		[ImplementationRequiresName(EngineType.Unity3D, "FixedUpdate")]
		void FixedUpdate();
	}
}
