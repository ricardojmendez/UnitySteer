using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Testity.BuildProcess
{
	public interface ITypeMemberParser
	{
		//TOOD: Out of data documentation

		/// <summary>
		/// Creates a collection of <see cref="IEnumerable{TMemberInfoType}"/> that reference the <see cref="TTypeToParse"/> members.
		/// These members will also be decorated by the <see cref="ExposeDataMemeberAttribute"/>.
		/// </summary>
		/// <typeparam name="TTypeToParse">Type to be parsed</typeparam>
		/// <returns>A collection of <typeparamref name="MemberInfoType"/></returns>
		IEnumerable<MemberInfo> Parse<TTypeToParse>(MemberTypes types)
				where TTypeToParse : class;

		/// <summary>
		/// Creates a collection of <see cref="IEnumerable{TMemberInfoType}"/> that reference the <see cref="TTypeToParse"/> members.
		/// These members will also be decorated by the <see cref="ExposeDataMemeberAttribute"/>.
		/// </summary>
		/// <returns>A collection of <typeparamref name="MemberInfoType"/></returns>
		IEnumerable<MemberInfo> Parse(MemberTypes types, Type typeToParse);
	}
}
