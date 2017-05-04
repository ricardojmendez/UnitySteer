using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testity.BuildProcess.Unity3D
{
	public interface IInitializationExpressionBuilder
	{
		IInitializationExpression Build(InitializationExpressionData data, string targetEngineComponentFieldName);
    }
}
