using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Testity.EngineComponents;

namespace Testity.EngineServices
{
	/// <summary>
	/// Implementer provides factory/creation services for <see cref="TCreationType"/> objects.
	/// Also makes the promise of managing dependencies for construction.
	/// </summary>
	/// <typeparam name="TCreationType">Type to be created.</typeparam>
	[EngineServiceInterface]
	public interface IDependencyInjectionManagedFactory<TCreationType>
		where TCreationType : class
	{
		TCreationType Create();

		IEnumerable<TCreationType> CreateMany(int count);
	}
}
