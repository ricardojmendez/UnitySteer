using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Testity.EngineComponents;
using Testity.EngineMath;

namespace Testity.EngineServices
{
	/// <summary>
	/// And adapter
	/// </summary>
	/// <typeparam name="TActualGameObjectType"></typeparam>
	public abstract class LifetimeManagedGameObjectFactoryServiceAdapter<TActualGameObjectType> : IEngineGameObjectFactoryService
		where TActualGameObjectType : class
	{
		/// <summary>
		/// Unregisteration service for <typeparamref name="TActualGameObjectType"/>s.
		/// </summary>
		protected readonly ILifetimeManagedEngineObjectRegister<IEngineGameObject, TActualGameObjectType> lifetimeManagerRegister;

		/// <summary>
		/// Creates an adapter abstract interface for implementers to implement for managed creation of <typeparamref name="TActualGameObjectType"/>.
		/// </summary>
		/// <param name="lifetimeRegister">Register to register <see cref="IEngineObject"/> adapters in.</param>
		public LifetimeManagedGameObjectFactoryServiceAdapter(ILifetimeManagedEngineObjectRegister<IEngineGameObject, TActualGameObjectType> lifetimeRegister)
		{
			if (lifetimeManagerRegister == null)
				throw new ArgumentNullException(nameof(lifetimeRegister), nameof(ILifetimeManagedEngineObjectRegister<IEngineGameObject, TActualGameObjectType>) + " must not be null for " + this.GetType());

			lifetimeManagerRegister = lifetimeRegister;
		}

		/// <summary>
		/// Creates a <see cref="IEngineGameObject"/> at the specified <see cref="Vector3{TMathType}"/> postion and <see cref="Quaternion{TMathType}"/> rotation and registers it internally with the adapter registry.
		/// </summary>
		/// <param name="position"><see cref="Vector3{TMathType}"/> position of the <see cref="IEngineGameObject"/> to be created.</param>
		/// <param name="rotation"><see cref="Quaternion{TMathType}"/> representing the rotation of the <see cref="IEngineGameObject"/> to be created.</param>
		/// <returns>A valid non-null <see cref="IEngineGameObject"/> at specified position and rotation.</returns>
		public IEngineGameObject Create(Vector3<float> position, Quaternion<float> rotation)
		{
			//Creates an instance of the actual engine gameobject and then registers it.
			//Once registeration is done it returns it for use. No consumer of the factory will know anything but of the creation.

			TActualGameObjectType concreteEngineObjectInstance = CreateActualEngineObject(position, rotation);

			if (concreteEngineObjectInstance == null)
				throw new InvalidOperationException(nameof(LifetimeManagedGameObjectFactoryServiceAdapter<TActualGameObjectType>) + " failed to produce a valid " + nameof(TActualGameObjectType) + " via method: " + nameof(CreateActualEngineObject));

			IEngineGameObject bridgedGameObject = CreateGameObjectAdapter(concreteEngineObjectInstance);

			if (bridgedGameObject == null)
				throw new InvalidOperationException(nameof(LifetimeManagedGameObjectFactoryServiceAdapter<TActualGameObjectType>) + " failed to produce a valid " + nameof(IEngineGameObject) + " vai method: " + nameof(CreateGameObjectAdapter));

			lifetimeManagerRegister.Register(bridgedGameObject, concreteEngineObjectInstance);

			return bridgedGameObject;
		}

		/// <summary>
		/// Creates a <see cref="IEnginePrefabedGameObject"/> at the specified <see cref="Vector3{TMathType}"/> postion and <see cref="Quaternion{TMathType}"/> rotation and registers it internally with the adapter registry.
		/// </summary>
		/// <param name="position"><see cref="Vector3{TMathType}"/> position of the <see cref="IEnginePrefabedGameObject"/> to be created.</param>
		/// <param name="rotation"><see cref="Quaternion{TMathType}"/> representing the rotation of the <see cref="IEngineGameObject"/> to be created.</param>
		/// <param name="prefab">Prefab information service.</param>
		/// <returns>A valid non-null <see cref="IEnginePrefabedGameObject"/> at specified position and rotation.</returns>
		public IEnginePrefabedGameObject Create(IEnginePrefab prefab, Vector3<float> position, Quaternion<float> rotation)
		{
			//Creates an instance of the actual engine gameobject and then registers it.
			//Once registeration is done it returns it for use. No consumer of the factory will know anything but of the creation.

			TActualGameObjectType concreteEngineObjectInstance = CreateActualEngineObject(position, rotation);

			if (concreteEngineObjectInstance == null)
				throw new InvalidOperationException(nameof(LifetimeManagedGameObjectFactoryServiceAdapter<TActualGameObjectType>) + " failed to produce a valid " + nameof(TActualGameObjectType) + " via method: " + nameof(CreateActualEngineObject));

			IEnginePrefabedGameObject bridgedGameObject = CreatePrefabbedGameObjectAdapter(prefab, concreteEngineObjectInstance);

			if (bridgedGameObject == null)
				throw new InvalidOperationException(nameof(LifetimeManagedGameObjectFactoryServiceAdapter<TActualGameObjectType>) + " failed to produce a valid " + nameof(IEnginePrefabedGameObject) + " vai method: " + nameof(CreatePrefabbedGameObjectAdapter));

			lifetimeManagerRegister.Register(bridgedGameObject, concreteEngineObjectInstance);

			return bridgedGameObject;
		}

		/// <summary>
		/// Creates the actual underlying GameObject type at the specified position and rotation.
		/// </summary>
		/// <param name="position">The <see cref="Vector3{TMathType}"/> position to create the <typeparamref name="TActualGameObjectType"/> object at.</param>
		/// <param name="rotation">The <see cref="Quaternion{TMathType}"/> rotation to create the <typeparamref name="TActualGameObjectType"/> object with.</param>
		/// <returns>A valid instance of <typeparamref name="TActualGameObjectType"/> with the specified transformation.</returns>
		protected abstract TActualGameObjectType CreateActualEngineObject(Vector3<float> position, Quaternion<float> rotation);

		/// <summary>
		/// Creates the actual underlying prefabbed GameObject type at the specified position and rotation.
		/// </summary>
		/// <param name="position">The <see cref="Vector3{TMathType}"/> position to create the <typeparamref name="TActualGameObjectType"/> object at.</param>
		/// <param name="rotation">The <see cref="Quaternion{TMathType}"/> rotation to create the <typeparamref name="TActualGameObjectType"/> object with.</param>
		/// <param name="prefab">Prefab information service.</param>
		/// <returns>A valid instance of <typeparamref name="TActualGameObjectType"/> with the specified transformation.</returns>
		protected abstract TActualGameObjectType CreateActualEngineObject(IEnginePrefab prefab, Vector3<float> position, Quaternion<float> rotation);

		/// <summary>
		/// Creates an adapter around the <typeparamref name="TActualGameObjectType"/> object instance for use externally.
		/// </summary>
		/// <param name="actualGameObject">Object to plug in to the adapter.</param>
		/// <returns>A valid adapter around the <typeparamref name="TActualGameObjectType"/> instance.</returns>
		protected abstract IEngineGameObject CreateGameObjectAdapter(TActualGameObjectType actualGameObject);

		/// <summary>
		/// Creates an adapter around the <typeparamref name="TActualGameObjectType"/> object instance and <see cref="IEnginePrefab"/> for use externally.
		/// </summary>
		/// <param name="actualGameObject">Object to plug in to the adapter.</param>
		/// <param name="prefab">Prefab information service.</param>
		/// <returns>A valid adapter around the <typeparamref name="TActualGameObjectType"/> instance.</returns>
		protected abstract IEnginePrefabedGameObject CreatePrefabbedGameObjectAdapter(IEnginePrefab prefab, TActualGameObjectType actualGameObject);
	}
}
