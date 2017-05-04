using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Testity.Common;
using Testity.EngineComponents;

namespace Testity.EngineComponents
{
	/// <summary>
	/// Implementer provides information about a Prefab instance.
	/// Based on Unity3D's: http://docs.unity3d.com/ScriptReference/Object.Instantiate.html
	/// </summary>
	[EngineComponentInterface]
	[EngineSerializableMapsToType(EngineType.Unity3D, "UnityEngine.GameObject, UnityEngine")]
	public interface IEnginePrefab
	{
		/// <summary>
		/// The name of the Prefab.
		/// </summary>
		string PrefabName { get; }

		/// <summary>
		/// Indicates if the Prefab service is available. It may not be if uninitialized by the user.
		/// </summary>
		bool isAvailable { get; }
	}
}
