using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Testity.EngineComponents;

namespace Testity.EngineServices
{
	/// <summary>
	/// Dictionary/map that explictly requires a custom <see cref="IEqualityComparer{TEngineObjectType}"/>. This is for mapping <typeparamref name="TEngineObjectType"/>s to references of <typeparamref name="TActualEngineObjectType"/>.
	/// </summary>
	/// <typeparam name="TEngineObjectType">Engine object adapter type.</typeparam>
	/// <typeparam name="TActualEngineObjectType">Actual game engine's object type.</typeparam>
	[Serializable]
	public class EngineObjectReferenceDictionary<TEngineObjectType, TActualEngineObjectType> : Dictionary<TEngineObjectType, TActualEngineObjectType>, IReadOnlyMapLookup<TEngineObjectType, TActualEngineObjectType>
		where TEngineObjectType : class, IEngineObject where TActualEngineObjectType : class
	{
		/// <summary>
		/// Initializes a dictionary with the default <see cref="IEqualityComparer{TEngineObjectType}"/>.
		/// </summary>
		public EngineObjectReferenceDictionary()
			: base()
		{

		}

		/// <summary>
		/// Initializes a dictionary with the default <see cref="IEqualityComparer{TEngineObjectType}"/>.
		/// <param name="initialSize">Intial size of the dictionary.</param>
		/// </summary>
		public EngineObjectReferenceDictionary(int initialSize)
			: base(initialSize)
		{

		}

		/// <summary>
		/// Initializes a dictionary with the custom <see cref="IEqualityComparer{TEngineObjectType}"/>.
		/// </summary>
		/// <param name="customEngineObjectEqualityComparer">Custom reference equality comparer.</param>
		public EngineObjectReferenceDictionary(IEqualityComparer<TEngineObjectType> customEngineObjectEqualityComparer)
			: base(customEngineObjectEqualityComparer)
		{

		}

		/// <summary>
		/// Initializes a dictionary with the custom <see cref="IEqualityComparer{TEngineObjectType}"/> with an intialize size.
		/// </summary>
		/// <param name="customEngineObjectEqualityComparer">Custom reference equality comparer.</param>
		/// <param name="initialSize">Intial size of the dictionary.</param>
		public EngineObjectReferenceDictionary(int initialSize, IEqualityComparer<TEngineObjectType> customEngineObjectEqualityComparer)
			: base(initialSize, customEngineObjectEqualityComparer)
		{

		}

		/// <summary>
		/// Attempts to register a reference to <typeparamref name="TActualEngineObjectType"/> with the key being <typeparamref name="TEngineObjectType"/>.
		/// </summary>
		/// <param name="keyToRegister">Key to store the reference under.</param>
		/// <param name="referenceToRegister">Reference to store.</param>
		/// <returns>Indicates if the object is stored.</returns>
		/// <exception cref="ArgumentNullException">Throws if either arg is null.</exception>
		public bool Register(TEngineObjectType keyToRegister, TActualEngineObjectType referenceToRegister)
		{
			if (keyToRegister == null)
				throw new ArgumentNullException(nameof(keyToRegister), "Key cannot be null.");

			if(referenceToRegister == null)
				throw new ArgumentNullException(nameof(referenceToRegister), "Value cannot be null.");

			if (!ContainsKey(keyToRegister))
				Add(keyToRegister, referenceToRegister);

			return true;
		}

		/// <summary>
		/// Attempts to lookup an instance of <see cref="TValue"/> using the specified <see cref="TKey"/>.
		/// </summary>
		/// <param name="key">Key value.</param>
		/// <returns>If found it returning the stored <see cref="TValue"/> instance. Otherwise null.</returns>
		/// <exception cref="ArgumentNullException">Will throw if key is null.</exception>
		public TActualEngineObjectType TryLookup(TEngineObjectType key)
		{
			if (ContainsKey(key))
				return this[key];
			else
				return null;
		}

		/// <summary>
		/// Attempts to unregister the <typeparamref name="TEngineObjectType"/> key. Returns the unregistered object if possible.
		/// </summary>
		/// <param name="objectToUnregister">Key to unregister.</param>
		/// <returns>A result object that indicates failure or success. May contain the <typeparamref name="TActualEngineObjectType"/> object.</returns>
		public UnregistrationResult<TActualEngineObjectType> TryUnregister(TEngineObjectType objectToUnregister)
		{
			if (ContainsKey(objectToUnregister))
			{
				TActualEngineObjectType value = this[objectToUnregister];

				//shouldn't ever fail
				if (!Remove(objectToUnregister))
					throw new InvalidOperationException("Failed to remove " + nameof(objectToUnregister) + " from map in " + GetType());

	return new UnregistrationResult<TActualEngineObjectType>(true, value);
			}		
			else
				return new UnregistrationResult<TActualEngineObjectType>(false, null);
		}
	}
}
