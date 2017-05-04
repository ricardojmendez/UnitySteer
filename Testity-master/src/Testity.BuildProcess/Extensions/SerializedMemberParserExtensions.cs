using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Testity.BuildProcess
{
	public static class SerializedMemberParserExtensions
	{
		public static SerializedMemberParser<TMemberInfoType> Generic<TMemberInfoType>(this SerializedMemberParser parser)
			where TMemberInfoType : MemberInfo
        {
			return new SerializedMemberParser<TMemberInfoType>();
		}
	}
}
