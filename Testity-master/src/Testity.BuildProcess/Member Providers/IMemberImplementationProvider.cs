using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testity.BuildProcess
{
	public interface IMemberImplementationProvider : ITypeImplementationProvider
	{
		SyntaxToken MemberName { get; }

		SyntaxTokenList Modifiers { get; }

		SyntaxList<AttributeListSyntax> Attributes { get; }
	}
}
