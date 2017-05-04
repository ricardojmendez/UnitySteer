using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Testity.BuildProcess.Unity3D
{
	public class InitializationExpressionData
	{
		public readonly string SourceFieldName;

		public readonly string DestinationFieldName;

		public readonly MemberTypes DestinationMemberType;

		public InitializationExpressionData(string sourFieldName, MemberTypes sourceMemberType, string destFieldName)
		{
			SourceFieldName = sourFieldName;
			DestinationFieldName = destFieldName;
			DestinationMemberType = sourceMemberType;
        }
	}
}
