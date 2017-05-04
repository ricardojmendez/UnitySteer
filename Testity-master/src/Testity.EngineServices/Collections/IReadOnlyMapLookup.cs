using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Testity.EngineServices
{
	/// <summary>
	/// Simple readonly dictionary lookup functionality.
	/// </summary>
	/// <typeparam name="TKey">Type of key.</typeparam>
	/// <typeparam name="TValue">Type of stored value.</typeparam>
	public interface IReadOnlyMapLookup<TKey, TValue>
	{
		/// <summary>
		/// Attempts to lookup an instance of <see cref="TValue"/> using the specified <see cref="TKey"/>.
		/// </summary>
		/// <param name="key">Key value.</param>
		/// <returns>If found it returning the stored <see cref="TValue"/> instance. Otherwise null.</returns>
		TValue TryLookup(TKey key);
	}
}
