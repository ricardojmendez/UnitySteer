using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Fasterflect;
using Microsoft.CodeAnalysis;

namespace Testity.BuildProcess.Unity3D
{
	public class UnityInitializationMethodImplementationProvider : IBlockBodyProvider
	{
		public BlockSyntax Block { get; private set; }

		public UnityInitializationMethodImplementationProvider(IEnumerable<IInitializationExpression> expressions)
		{
			Block = SyntaxFactory.Block();

#if DEBUG || DEBUGBUILD
			if (expressions == null)
				throw new ArgumentNullException(nameof(expressions), "Expressions collection cannot be null in " + typeof(UnityInitializationMethodImplementationProvider) + " as it is required to build the init method.");
#endif

			//Add each expression as a statement to the block
			//WARNING: Do not use foreach. At least not with WithStatements. It'll override the previous Block staterments each time.
			Block = Block.AddStatements(expressions.Select(x => x.Statement).ToArray());

			/*//foreach serialized field in the MonoBehaviour we take a look at it, add a statement for setting it via reflection
			//find an adapter for the source to dest Type and create an instance of it to initialize the EngineScriptComponent
			foreach(InitializationExpressionData data in expressions)
			{
				string fasterFlectMethodName = data.DestinationMemberType == System.Reflection.MemberTypes.Field ?
					@"SetFieldValue" : @"SetPropertyValue"; //can't use nameof with extension method I think. Will

				//genertaed using: http://roslynquoter.azurewebsites.net/ do not attempt to read. Ask Andrew
				Block = Block.WithStatements(SyntaxFactory.SingletonList<StatementSyntax>(SyntaxFactory.ExpressionStatement(
								SyntaxFactory.InvocationExpression(
									SyntaxFactory.MemberAccessExpression(
										SyntaxKind.SimpleMemberAccessExpression,
										SyntaxFactory.IdentifierName(
											targetEngineComponentFieldName),
										SyntaxFactory.IdentifierName(
											fasterFlectMethodName)) //method to call. We're using fasterflect cached reflection
									.WithOperatorToken(
										SyntaxFactory.Token(
											SyntaxKind.DotToken)))
								.WithArgumentList(
									SyntaxFactory.ArgumentList(
										SyntaxFactory.SeparatedList<ArgumentSyntax>(
											new SyntaxNodeOrToken[]{
												SyntaxFactory.Argument(
													SyntaxFactory.IdentifierName(
														data.DestinationFieldName)), //changed to destination field name
												SyntaxFactory.Token(
													SyntaxKind.CommaToken),
												SyntaxFactory.Argument(
													SyntaxFactory.ObjectCreationExpression(
														SyntaxFactory.IdentifierName(
															data.AdapterData.AdapterType.FullName)) //changed to create a new adapter type
													.WithNewKeyword(
														SyntaxFactory.Token(
															SyntaxKind.NewKeyword))
													.WithArgumentList(
														SyntaxFactory.ArgumentList(
															SyntaxFactory.SingletonSeparatedList<ArgumentSyntax>(
																SyntaxFactory.Argument(
																	SyntaxFactory.IdentifierName(
																		data.SourceFieldName)))) //changed to be the source field name
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
											SyntaxKind.CloseParenToken))))));
											*/
		}
	}
}
