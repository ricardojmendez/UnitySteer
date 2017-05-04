using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testity.BuildProcess.Unity3D
{
	public class ActionDelegateTypeExclusion : ITypeExclusion
	{
		private readonly IEnumerable<Type> typesToExclude;

		public ActionDelegateTypeExclusion(IEnumerable<Type> actionTypesToExclude)
		{
			if (actionTypesToExclude == null)
				throw new ArgumentNullException(nameof(actionTypesToExclude), "Action type collection cannot be null.");

			typesToExclude = actionTypesToExclude;
		}

		public bool isExcluded(Type t)
		{
			if (t == null)
				throw new ArgumentNullException(nameof(t), "Type to check cannot be null.");

			return typesToExclude.Contains(t);
        }
	}
}
