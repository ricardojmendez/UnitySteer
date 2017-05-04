using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testity.BuildProcess.Unity3D
{
	public interface ITypeExclusion
	{
		bool isExcluded(Type t);
	}
}
