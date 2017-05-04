using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Testity.EngineComponents;
using Testity.EngineMath;

namespace Testity.EngineServices
{
	/// <summary>
	/// Extends the functionality and API of <see cref="IEngineGameObjectFactoryService"/>
	/// </summary>
	public static class IEngineGameObjectFactoryExtensions
	{
		/// <summary>
		/// Creates a default <see cref="IEngineGameObject"/> at the specified <see cref="Vector3{TMathType}"/> postion and euler <see cref="Vector3{TMathType}"/> rotation.
		/// </summary>
		/// <param name="position"><see cref="Vector3{TMathType}"/> position of the <see cref="IEnginePrefabedGameObject"/> to be created.</param>
		/// <param name="rotation"><see cref="Vector3{TMathType}"/> representing the euler vector rotation of the <see cref="IEnginePrefabedGameObject"/> to be created.</param>
		/// <param name="factoryService">Factory instace that is being extended via this extension method.</param>
		/// <returns>A valid non-null <see cref="IEngineGameObject"/> at specified position and rotation.</returns>
		public static IEngineGameObject Create<TGameObjectType>(this IEngineGameObjectFactory<TGameObjectType> factoryService, Vector3<float> position, Vector3<float> rotation)
			where TGameObjectType : class, IEngineGameObject
		{
			return factoryService.Create(position, rotation.Euler());
		}

		/// <summary>
		/// Creates a default <see cref="IEngineGameObject"/> at the specified <see cref="Vector3{TMathType}"/> postion and no rotation.
		/// </summary>
		/// <param name="position"><see cref="Vector3{TMathType}"/> position of the <see cref="IEnginePrefabedGameObject"/> to be created.</param>
		/// <param name="factoryService">Factory instace that is being extended via this extension method.</param>
		/// <returns>A valid non-null <see cref="IEngineGameObject"/> at specified position and no rotation.</returns>
		public static IEngineGameObject Create<TGameObjectType>(this IEngineGameObjectFactory<TGameObjectType> factoryService, Vector3<float> position)
			where TGameObjectType : class, IEngineGameObject
		{
			return factoryService.Create(position, Quaternion<float>.identity);
		}

		/// <summary>
		/// Creates a default <see cref="IEngineGameObject"/> at the origin with no rotation.
		/// </summary>
		/// <param name="factoryService">Factory instace that is being extended via this extension method.</param>
		/// <returns>A valid non-null <see cref="IEngineGameObject"/> at specified position and default rotation.</returns>
		public static IEngineGameObject Create<TGameObjectType>(this IEngineGameObjectFactory<TGameObjectType> factoryService)
			where TGameObjectType : class, IEngineGameObject
		{
			return factoryService.Create(Vector3<float>.zero, Quaternion<float>.identity);
		}
	}
}
