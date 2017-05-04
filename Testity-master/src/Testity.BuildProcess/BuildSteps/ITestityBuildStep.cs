using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testity.BuildProcess
{
	public interface ITestityBuildStep
	{
		void Process(IClassBuilder builder, Type typeToParse);
	}
}
