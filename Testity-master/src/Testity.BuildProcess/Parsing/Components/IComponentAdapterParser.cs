using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testity.BuildProcess
{
	public interface IComponentAdapterParser
	{
		IEnumerable<IComponentAdapterData> ParseFor(Type typeToAdapt, Type targetTypeToAdaptTo);
	}
}
