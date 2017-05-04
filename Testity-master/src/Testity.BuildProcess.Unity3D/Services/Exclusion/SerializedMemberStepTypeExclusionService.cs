using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testity.BuildProcess.Unity3D
{
	public class SerializedMemberStepTypeExclusionService : ITypeExclusion
	{
		private IEnumerable<ITypeExclusion> typeExclusionChain;

		public SerializedMemberStepTypeExclusionService(IEnumerable<ITypeExclusion> exclusionChain)
		{
			if (exclusionChain == null)
				throw new ArgumentNullException(nameof(exclusionChain), "Cannot set a null set of services for exclusion.");

			typeExclusionChain = exclusionChain;
        }

		public SerializedMemberStepTypeExclusionService()
			: this(Enumerable.Empty<ITypeExclusion>())
		{

		}

		public bool isExcluded(Type t)
		{
			foreach (ITypeExclusion e in typeExclusionChain)
				if (e.isExcluded(t))
					return true;

			return false;
		}

		public SerializedMemberStepTypeExclusionService AddExclusionRules(params ITypeExclusion[] e)
		{
			if (e == null)
				throw new ArgumentNullException(nameof(e), "Cannot add null exclusion services to main service.");

			typeExclusionChain = typeExclusionChain.Concat(e);

			return this;
		}
	}
}
