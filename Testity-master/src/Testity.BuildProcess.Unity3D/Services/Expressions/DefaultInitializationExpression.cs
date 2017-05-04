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
	public class DefaultInitializationExpression : IInitializationExpression
	{
		public ExpressionStatementSyntax Statement { get; private set; }

		public DefaultInitializationExpression(InitializationExpressionData data, string targetEngineComponentFieldName)
		{
            string fasterFlectMethodName = data.DestinationMemberType == System.Reflection.MemberTypes.Field ?
					@"SetFieldValue" : @"SetPropertyValue"; //can't use nameof with extension method I think. Will

			//generated with: http://roslynquoter.azurewebsites.net/ do not try to read
			Statement = SyntaxFactory.ExpressionStatement(
									SyntaxFactory.InvocationExpression(
										SyntaxFactory.MemberAccessExpression(
											SyntaxKind.SimpleMemberAccessExpression,
											SyntaxFactory.IdentifierName(
												targetEngineComponentFieldName), //target field
											SyntaxFactory.IdentifierName(
												fasterFlectMethodName))
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
														SyntaxFactory.InvocationExpression(
															SyntaxFactory.MemberAccessExpression(
																SyntaxKind.SimpleMemberAccessExpression,
																SyntaxFactory.IdentifierName(
																	data.SourceFieldName),
																SyntaxFactory.IdentifierName(
																	@"ToEngineType"))
															.WithOperatorToken(
																SyntaxFactory.Token(
																	SyntaxKind.DotToken)))
														.WithArgumentList(
															SyntaxFactory.ArgumentList()
															.WithOpenParenToken(
																SyntaxFactory.Token(
																	SyntaxKind.OpenParenToken))
															.WithCloseParenToken(
																SyntaxFactory.Token(
																	SyntaxKind.CloseParenToken))))}))
										.WithOpenParenToken(
											SyntaxFactory.Token(
												SyntaxKind.OpenParenToken))
										.WithCloseParenToken(
											SyntaxFactory.Token(
												SyntaxKind.CloseParenToken))));
        }
	}
}
