using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Testity.EngineComponents;

namespace Testity.EngineServices
{

	/// <summary>
	/// Implementer provides functionality for destroying and cleaning up of <typeparamref name="TEngineObjectType"/>s.
	/// Based loosely on Unity3D's: http://docs.unity3d.com/ScriptReference/Object.Destroy.html
	/// </summary>
	[EngineServiceInterface]
	public interface IEngineObjectDestructionService<TEngineObjectType>
		where TEngineObjectType : class, IEngineObject
	{
		/// <summary>
		/// Destroys the <typeparamref name="TEngineObjectType"/> requested.
		/// Based on Unity3D's: http://docs.unity3d.com/ScriptReference/Object.Destroy.html
		/// </summary>
		/// <param name="toDestroy"><typeparamref name="TEngineObjectType"/> to destroy.</param>
		/// <returns>Indicates if destruction was sucessful.</returns>
		bool Destroy(TEngineObjectType toDestroy);

		/// <summary>
		/// Destroys the <typeparamref name="TEngineObjectType"/> requested approximately time seconds after this call.
		/// Based on Unity3D's: http://docs.unity3d.com/ScriptReference/Object.Destroy.html overload
		/// <param name="time">Time in seconds to approximately delay the destruction of the object.</param>
		/// <param name="toDestroy"><typeparamref name="TEngineObjectType"/> to destroy.</param>
		/// </summary>
		/// <returns>Indicates if destruction was sucessful.</returns>
		bool DestroyDelayed(TEngineObjectType toDestroy, float time);
	}
}
