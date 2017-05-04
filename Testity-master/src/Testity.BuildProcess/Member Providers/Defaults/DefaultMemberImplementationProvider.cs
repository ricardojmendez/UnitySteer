using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;

namespace Testity.BuildProcess
{
	public class DefaultMemberImplementationProvider : IMemberImplementationProvider
	{
		public SyntaxList<AttributeListSyntax> Attributes { get; private set; }

		public SyntaxToken MemberName { get; private set; }

		public SyntaxTokenList Modifiers { get; private set; }

		public TypeSyntax Type { get; private set; }

		public DefaultMemberImplementationProvider(Type memberType, MemberImplementationModifier modifiers, string name)
		{
			MemberName = SyntaxFactory.Identifier(name);
			Modifiers = SyntaxFactory.TokenList(modifiers.ToSyntaxKind().Select(x => SyntaxFactory.Token(x)));
			Type = SyntaxFactory.ParseTypeName(memberType != typeof(void) ? memberType.FullName : "void"); //we can't use void fullname or else we end up with System.Void which is rejected
		}
	}
}
