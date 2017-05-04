using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Testity.BuildProcess.Unity3D
{
	public interface IInitializationExpressionBuilderProvider
	{
		IInitializationExpressionBuilder FromReflectionData(Type sourceInfo, Type destInfo);
	}
}
