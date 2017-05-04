using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Fasterflect;
using Testity.Common;

namespace Testity.BuildProcess
{
	public class SerializedMemberParser : ITypeMemberParser
	{
		public SerializedMemberParser()
		{

		}

		/// <summary>
		/// Creates a collection of <see cref="IEnumerable{TMemberInfoType}"/> that reference the <see cref="TTypeToParse"/> members.
		/// These members will also be decorated by the <see cref="ExposeDataMemeberAttribute"/>.
		/// </summary>
		/// <typeparam name="TTypeToParse">Type to be parsed</typeparam>
		/// <returns>A collection of <typeparamref name="MemberInfoType"/></returns>
		public IEnumerable<MemberInfo> Parse<TTypeToParse>(MemberTypes types)
			where TTypeToParse : class
		{
			return typeof(TTypeToParse).MembersWith<ExposeDataMemeberAttribute>(types, Flags.InstanceAnyVisibility); //get all members from current and base classes
		}

		/// <summary>
		/// Creates a collection of <see cref="IEnumerable{TMemberInfoType}"/> that reference the <see cref="TTypeToParse"/> members.
		/// These members will also be decorated by the <see cref="ExposeDataMemeberAttribute"/>.
		/// </summary>
		/// <returns>A collection of <typeparamref name="MemberInfoType"/></returns>
		public IEnumerable<MemberInfo> Parse(MemberTypes types, Type typeToParse)
		{
			return typeToParse.MembersWith<ExposeDataMemeberAttribute>(types, Flags.InstanceAnyVisibility); //get all members from current and base classes
		}
	}
}
