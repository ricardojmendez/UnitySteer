using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Testity.Common;

namespace Testity.EngineComponents
{
	/// <summary>
	/// Implementer is a Prefabed <see cref="IEngineGameObject"/> that provides additional information and services
	/// about the prefab through this interface.
	/// Not based on anything in Unity3D.
	/// </summary>
	[EngineComponentInterface]
	public interface IEnginePrefabedGameObject : IEngineGameObject
	{
		string PrefabName { get; }
	}
}
