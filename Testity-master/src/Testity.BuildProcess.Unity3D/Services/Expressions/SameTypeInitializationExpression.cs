using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;

namespace Testity.BuildProcess.Unity3D
{
	public class SameTypeInitialization : IInitializationExpression
	{
		public ExpressionStatementSyntax Statement { get; private set; }

		public SameTypeInitialization(InitializationExpressionData data, string targetEngineComponentFieldName)
		{
			string fasterFlectMethodName = data.DestinationMemberType == System.Reflection.MemberTypes.Field ?
				   @"SetFieldValue" : @"SetPropertyValue"; //can't use nameof with extension method I think. Will

			Statement = SyntaxFactory.ExpressionStatement(
									SyntaxFactory.InvocationExpression(
										SyntaxFactory.MemberAccessExpression(
											SyntaxKind.SimpleMemberAccessExpression,
											SyntaxFactory.IdentifierName(
												targetEngineComponentFieldName), //target field
											SyntaxFactory.IdentifierName(
												fasterFlectMethodName)) //reflection method
										.WithOperatorToken(
											SyntaxFactory.Token(
												SyntaxKind.DotToken)))
									.WithArgumentList(
										SyntaxFactory.ArgumentList(
											SyntaxFactory.SeparatedList<ArgumentSyntax>(
												new SyntaxNodeOrToken[]{
													SyntaxFactory.Argument(
														SyntaxFactory.LiteralExpression(
															SyntaxKind.StringLiteralExpression,
															SyntaxFactory.Literal(
																SyntaxFactory.TriviaList(),
																@"""" + data.DestinationFieldName + @"""",
																@"""" + data.DestinationFieldName + @"""",
																SyntaxFactory.TriviaList()))),
													SyntaxFactory.Token(
														SyntaxKind.CommaToken),
													SyntaxFactory.Argument(
														SyntaxFactory.IdentifierName(
															data.SourceFieldName))}))
										.WithOpenParenToken(
											SyntaxFactory.Token(
												SyntaxKind.OpenParenToken))
										.WithCloseParenToken(
											SyntaxFactory.Token(
												SyntaxKind.CloseParenToken))))
								.WithSemicolonToken(
									SyntaxFactory.Token(
										SyntaxKind.SemicolonToken));
		}
	}
}
