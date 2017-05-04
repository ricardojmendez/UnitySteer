using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Testity.EngineComponents
{
	/// <summary>
	/// Implementer provides functionality for getting and setting the active state.
	/// </summary>
	public interface IEngineActivatable
	{
		/// <summary>
		/// Indicates if the implementer is itself active.
		/// </summary>
		bool ActiveSelf { get; set; }

		/// <summary>
		/// Indicates if the implementer and and parents it's nested within are active; thus being absolutely active.
		/// Worst Case: O(n) where n is the number of upper level objects the implementer is nested within.
		/// Best Case: O(1) if cacheing is implemented. No promise is made for cacheing.
		/// </summary>
		bool ActiveAbsolute { get; }
	}
}
