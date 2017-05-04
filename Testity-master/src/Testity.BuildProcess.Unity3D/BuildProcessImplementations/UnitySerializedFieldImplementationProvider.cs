using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Testity.Common.Unity3D;
using Microsoft.CodeAnalysis.CSharp;

namespace Testity.BuildProcess.Unity3D
{
	public class UnitySerializedFieldImplementationProvider : IMemberImplementationProvider
	{
		public SyntaxToken MemberName { get; private set; }

		public TypeSyntax Type { get; private set; }

		public SyntaxTokenList Modifiers { get; private set; }

		public SyntaxList<AttributeListSyntax> Attributes { get; private set; }

		public UnitySerializedFieldImplementationProvider(string memberName, Type typeOfMember, WiredToAttribute wiredAttribute)
		{
			MemberName = SyntaxFactory.Identifier(memberName);
			Type = SyntaxFactory.ParseName(typeOfMember.FullName);

			//These modifiers are the same for all unity members. We make them private because we've no reason to do otherwise
			//Modifiers: Private
			Modifiers = SyntaxFactory.TokenList(MemberImplementationModifier.Private.ToSyntaxKind().Select(x => SyntaxFactory.Token(x)));

			//Unity fields require two attributes.
			//Attributes: SerializeField and WiredToAttribute
			Attributes = GenerateUnityAttributes(wiredAttribute);
		}

		public UnitySerializedFieldImplementationProvider(string memberName, string fullMemberName, WiredToAttribute wiredAttribute)
		{
			MemberName = SyntaxFactory.Identifier(memberName);
			Type = SyntaxFactory.ParseName(fullMemberName);

			//These modifiers are the same for all unity members. We make them private because we've no reason to do otherwise
			//Modifiers: Private
			Modifiers = SyntaxFactory.TokenList(MemberImplementationModifier.Private.ToSyntaxKind().Select(x => SyntaxFactory.Token(x)));

			//Unity fields require two attributes.
			//Attributes: SerializeField and WiredToAttribute
			Attributes = GenerateUnityAttributes(wiredAttribute);
		}

		private SyntaxList<AttributeListSyntax> GenerateUnityAttributes(WiredToAttribute wiredAttribute)
		{

			//Code generated from: http://roslynquoter.azurewebsites.net/
			//This is NOT human written. Don't try to read it
			return SyntaxFactory.SingletonList<AttributeListSyntax>(
				SyntaxFactory.AttributeList(
					SyntaxFactory.SeparatedList<AttributeSyntax>(
						new SyntaxNodeOrToken[]{
							SyntaxFactory.Attribute(
								SyntaxFactory.QualifiedName(
									SyntaxFactory.IdentifierName(
										@"UnityEngine"),
									SyntaxFactory.IdentifierName(
										@"SerializeField"))
								.WithDotToken(
									SyntaxFactory.Token(
										SyntaxKind.DotToken)))
							.WithArgumentList(
								SyntaxFactory.AttributeArgumentList()
								.WithOpenParenToken(
									SyntaxFactory.Token(
										SyntaxKind.OpenParenToken))
								.WithCloseParenToken(
									SyntaxFactory.Token(
										SyntaxKind.CloseParenToken))),
							SyntaxFactory.Token(
								SyntaxKind.CommaToken),
							SyntaxFactory.Attribute(
								SyntaxFactory.QualifiedName(
									SyntaxFactory.QualifiedName(
										SyntaxFactory.QualifiedName(
											SyntaxFactory.IdentifierName(
												@"Testity"),
											SyntaxFactory.IdentifierName(
												@"Common"))
										.WithDotToken(
											SyntaxFactory.Token(
												SyntaxKind.DotToken)),
										SyntaxFactory.IdentifierName(
											@"Unity3D"))
									.WithDotToken(
										SyntaxFactory.Token(
											SyntaxKind.DotToken)),
									SyntaxFactory.IdentifierName(
										@"WiredToAttribute"))
								.WithDotToken(
									SyntaxFactory.Token(
										SyntaxKind.DotToken)))
							.WithArgumentList(
								SyntaxFactory.AttributeArgumentList(
									SyntaxFactory.SeparatedList<AttributeArgumentSyntax>(
										new SyntaxNodeOrToken[]{
											SyntaxFactory.AttributeArgument(
												SyntaxFactory.MemberAccessExpression(
													SyntaxKind.SimpleMemberAccessExpression,
													SyntaxFactory.MemberAccessExpression(
														SyntaxKind.SimpleMemberAccessExpression,
														SyntaxFactory.MemberAccessExpression(
															SyntaxKind.SimpleMemberAccessExpression,
															SyntaxFactory.IdentifierName(
																@"System"),
															SyntaxFactory.IdentifierName(
																@"Reflection"))
														.WithOperatorToken(
															SyntaxFactory.Token(
																SyntaxKind.DotToken)),
														SyntaxFactory.IdentifierName(
															@"MemberTypes"))
													.WithOperatorToken(
														SyntaxFactory.Token(
															SyntaxKind.DotToken)),
													SyntaxFactory.IdentifierName(
														wiredAttribute.WiredMemberType.ToString())) //modified from the auto-generated source. Inserts membertype in
												.WithOperatorToken(
													SyntaxFactory.Token(
														SyntaxKind.DotToken))),
											SyntaxFactory.Token(
												SyntaxKind.CommaToken),
											SyntaxFactory.AttributeArgument(
												SyntaxFactory.LiteralExpression(
													SyntaxKind.StringLiteralExpression,
													SyntaxFactory.Literal(wiredAttribute.WiredMemberName))),

											//type it is wired to. Hand written
											SyntaxFactory.Token(
												SyntaxKind.CommaToken),

											SyntaxFactory.AttributeArgument(
												SyntaxFactory.LiteralExpression(
													SyntaxKind.StringLiteralExpression,
													SyntaxFactory.Literal(TypeToLoadableString(wiredAttribute.TypeWiredTo)))) //wiredAttribute.TypeWiredTo.ToString() + ", " + wiredAttribute.TypeWiredTo.Assembly.GetName().Name)))//wiredAttribute.TypeWiredTo.AssemblyQualifiedName.Remove(wiredAttribute.TypeWiredTo.AssemblyQualifiedName.TakeWhile(c => (two -= (c == ',' ? 1 : 0)) > 0).Count()))))//remove everything after the assembly. We can't have versioning info
											//handwritten ended
                                        }))
								.WithOpenParenToken(
									SyntaxFactory.Token(
										SyntaxKind.OpenParenToken))
								.WithCloseParenToken(
									SyntaxFactory.Token(
										SyntaxKind.CloseParenToken)))}))
				.WithOpenBracketToken(
					SyntaxFactory.Token(
						SyntaxKind.OpenBracketToken))
				.WithCloseBracketToken(
					SyntaxFactory.Token(
						SyntaxKind.CloseBracketToken)));
        }

		//Converts a Type into a string that can load the type. Contains no version or public key information for the assembly.
		private string TypeToLoadableString(Type t)
		{
			if (t.IsGenericType)
			{
				StringBuilder genericTypeArgBuilder = new StringBuilder();

				int count = 0;
				int useCommaAfter = 0;
				foreach (Type gTypeArg in t.GetGenericArguments())
				{
					//add a type enclused in brackets
					genericTypeArgBuilder.AppendFormat("{0}{1}{2}", "[", TypeToLoadableString(gTypeArg), "]");

					//after the first element we need to add a comma but not to the last one
					if (useCommaAfter <= count && (count + 1 < t.GetGenericArguments().Count()))
						genericTypeArgBuilder.Append(",");

					count++;
				}
					

				StringBuilder builder = new StringBuilder();
				builder.AppendFormat("{0}.{1}[{2}], {3}", t.Namespace, t.Name, genericTypeArgBuilder.ToString(), t.Assembly.GetName().Name);

				return builder.ToString();
			}
			else
			{
				StringBuilder builder = new StringBuilder();
				builder.AppendFormat("{0}.{1}, {2}", t.Namespace, t.Name, t.Assembly.GetName().Name);
				return builder.ToString();
			}
				
        }
	}
}
