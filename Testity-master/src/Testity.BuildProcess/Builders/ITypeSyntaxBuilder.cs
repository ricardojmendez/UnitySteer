using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testity.BuildProcess
{
	public interface ITypeSyntaxBuilder
	{
		TypeSyntax GenerateFrom(Type t);

		TypeSyntax GenerateFrom<TType>();

		IEnumerable<TypeSyntax> GenerateFrom(IEnumerable<Type> t);
	}
}
