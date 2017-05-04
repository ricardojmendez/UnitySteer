using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testity.BuildProcess
{
	[Flags]
	public enum MemberImplementationModifier : int
	{
		Private = 1 << 1,
		Public = 1 << 2,
		Virtual = 1 << 3,
		Override = 1 << 4,
		Static = 1 << 5,
		Sealed = 1 << 6,
		Protected = 1 << 7
	}
}
