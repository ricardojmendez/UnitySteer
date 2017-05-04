using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Testity.EngineComponents;

namespace Testity.EngineServices
{
	/// <summary>
	/// Implementer provides registeration services for <typeparamref name="TEngineObjectType"/> key types and <see cref="TUnderlyingObjectReferenceType"/> types.
	/// </summary>
	/// <typeparam name="TEngineObjectType">Object type that acts as a key.</typeparam>
	/// <typeparam name="TUnderlyingObjectReferenceType">The actual object type the lifetime manager manages.</typeparam>
	public interface ILifetimeManagedEngineObjectRegister<TEngineObjectType, TUnderlyingObjectReferenceType> : IReadOnlyMapLookup<TEngineObjectType, TUnderlyingObjectReferenceType>
		where TEngineObjectType : class, IEngineObject where TUnderlyingObjectReferenceType : class
	{
		/// <summary>
		/// Attempts to register a reference to <see cref="TUnderlyingObjectReferenceType"/> with the key being <typeparamref name="TEngineObjectType"/>.
		/// </summary>
		/// <param name="objectToRegister">Key to store the reference under.</param>
		/// <param name="referenceToRegister">Reference to store.</param>
		/// <returns>Indicates if the object is stored.</returns>
		bool Register(TEngineObjectType objectToRegister, TUnderlyingObjectReferenceType referenceToRegister);

		/// <summary>
		/// Attempts to unregister the <typeparamref name="TEngineObjectType"/> key. Returns the unregistered object if possible.
		/// </summary>
		/// <param name="objectToUnregister">Key to unregister.</param>
		/// <returns>A result object that indicates failure or success. May contain the <see cref="TUnderlyingObjectReferenceType"/> object.</returns>
		UnregistrationResult<TUnderlyingObjectReferenceType> TryUnregister(TEngineObjectType objectToUnregister);
	}
}
