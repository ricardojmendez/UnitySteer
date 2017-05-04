using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Testity.EngineComponents;

namespace Testity.EngineServices
{
	/// <summary>
	/// Abstract adapter class that provides lifetime managed destruction services for a specific <typeparamref name="TActualGameObjectType"/>.
	/// </summary>
	/// <typeparam name="TActualGameObjectType">Type of the underlying engines GameObject.</typeparam>
	public abstract class LifetimeManagedGameObjectDestructionServiceAdapter<TActualGameObjectType> : IEngineObjectDestructionService<IEngineGameObject>
	where TActualGameObjectType : class
	{
		/// <summary>
		/// Registeration service for <typeparamref name="TActualGameObjectType"/>s.
		/// </summary>
		private readonly ILifetimeManagedEngineObjectRegister<IEngineGameObject, TActualGameObjectType> lifetimeManagerRegister;

		/// <summary>
		/// Creates a managed destruction service based around the specified registry.
		/// </summary>
		/// <param name="register">Registry for object to destruct.</param>
		public LifetimeManagedGameObjectDestructionServiceAdapter(ILifetimeManagedEngineObjectRegister<IEngineGameObject, TActualGameObjectType> register)
		{
			lifetimeManagerRegister = register;
		}

		/// <summary>
		/// Destroys the <see cref="IEngineGameObject"/> requested.
		/// Based on Unity3D's: http://docs.unity3d.com/ScriptReference/Object.Destroy.html
		/// </summary>
		/// <param name="toDestroy"><see cref="IEngineGameObject"/> to destroy.</param>
		/// <returns>Indicates if destruction was sucessful.</returns>
		public bool Destroy(IEngineGameObject toDestroy)
		{
			UnregistrationResult<TActualGameObjectType> result = lifetimeManagerRegister.TryUnregister(toDestroy);

			if (!result.Success)
				return false;

			DestroyObject(result.Value);

			return true;
		}

		/// <summary>
		/// Destroys the <see cref="IEngineGameObject"/> requested approximately time seconds after this call.
		/// Based on Unity3D's: http://docs.unity3d.com/ScriptReference/Object.Destroy.html overload
		/// <param name="time">Time in seconds to approximately delay the destruction of the object.</param>
		/// <param name="toDestroy"><see cref="IEngineGameObject"/> to destroy.</param>
		/// </summary>
		/// <returns>Indicates if destruction was sucessful.</returns>
		public bool DestroyDelayed(IEngineGameObject toDestroy, float time)
		{
			//Don't unregister it yet
			TActualGameObjectType result = lifetimeManagerRegister.TryLookup(toDestroy);

			if (result == null)
				return false;

			//We can't really unregister this value. We have to force the underlying engine to handle the unregistering of the object when its destruction is done.
			DestroyObject(result, time, () => lifetimeManagerRegister.TryUnregister(toDestroy));

			return true;
		}

		/// <summary>
		/// Destroys the actual <typeparamref name="TActualGameObjectType"/> reference/object specified.
		/// </summary>
		/// <param name="toDestroy"><typeparamref name="TActualGameObjectType"/> instance to destroy.</param>
		protected abstract void DestroyObject(TActualGameObjectType toDestroy);

		/// <summary>
		/// Destroys the actual <typeparamref name="TActualGameObjectType"/> reference/object specified by the delayed float seconds.
		/// </summary>
		/// <param name="toDestroy"><typeparamref name="TActualGameObjectType"/> instance to destroy.</param>
		/// <param name="time">Time in seconds to delay the destruction by.</param>
		/// <param name="onObjectWasDestroyed">Callback for destroyed object. Should be invoked immediately after destruction.</param>
		protected abstract void DestroyObject(TActualGameObjectType toDestroy, float time, Action onObjectWasDestroyed);
	}
}
