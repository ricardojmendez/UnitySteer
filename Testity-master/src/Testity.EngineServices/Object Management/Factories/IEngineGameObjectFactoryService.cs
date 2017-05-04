using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Testity.EngineComponents;
using Testity.EngineMath;

namespace Testity.EngineServices
{
	/// <summary>
	/// Implementer provides <see cref="IEngineGameObject"/> and <see cref="IEnginePrefabedGameObject"/> creation functionality.
	/// Based on Unity3D's: http://docs.unity3d.com/ScriptReference/Object.Instantiate.html
	/// </summary>
	public interface IEngineGameObjectFactoryService : IEngineGameObjectFactory<IEngineGameObject>
	{
		//MS says don't do this but consumers of this API will find the injection of this type more preferable.

		/// <summary>
		/// Creates a <see cref="IEnginePrefabedGameObject"/> at the specified <see cref="Vector3{TMathType}"/> postion and <see cref="Quaternion{TMathType}"/> rotation.
		/// </summary>
		/// <param name="position"><see cref="Vector3{TMathType}"/> position of the <see cref="IEnginePrefabedGameObject"/> to be created.</param>
		/// <param name="rotation"><see cref="Quaternion{TMathType}"/> representing the rotation of the <see cref="IEnginePrefabedGameObject"/> to be created.</param>
		/// <param name="prefab">Prefab information service.</param>
		/// <returns>A valid non-null <see cref="IEnginePrefabedGameObject"/> at specified position and rotation.</returns>
		IEnginePrefabedGameObject Create(IEnginePrefab prefab, Vector3<float> position, Quaternion<float> rotation);
	}
}
