using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testity.Common;
using Testity.EngineComponents;

namespace Testity.BuildProcess.Tests
{
	[TestFixture]
	public static class ClassBuilderTests
	{
		[Test(Author = "Andrew Blakely", Description = "Tests the Rosyln compilation addition of using statements TestityClassBuilder.", TestOf = typeof(TestityClassBuilder))]
		public static void Test_TestityClassBuilder_TestAddedField()
		{
			//arrange
			TestityClassBuilder scriptBuilder = new TestityClassBuilder(typeof(EngineScriptComponent).Name + "Script");
			Mock<IMemberImplementationProvider> implementationProvider = BuildMemberImplementationMock("testField", typeof(EngineScriptComponent), MemberImplementationModifier.Private, new Type[] { typeof(ExposeDataMemeberAttribute) });

			//act
			scriptBuilder.AddClassField(implementationProvider.Object);

			//assert
			Assert.IsTrue(scriptBuilder.Compile().Contains("private " + typeof(EngineScriptComponent).FullName + " testField"));
			Assert.IsTrue(scriptBuilder.Compile().Contains("[" + typeof(ExposeDataMemeberAttribute).FullName+ "]"));
        }

		[Test(Author = "Andrew Blakely", Description = "Tests the Rosyln compilation adding of a base class with TestityClassBuilder.", TestOf = typeof(TestityClassBuilder))]
		public static void Test_TestityClassBuilder_Test_Adding_Base_Class()
		{
			//arrange
			TestityClassBuilder scriptBuilder = new TestityClassBuilder(typeof(EngineScriptComponent).Name + "Script");

			//act
			scriptBuilder.AddBaseClass<EngineScriptComponent>(new DefaultTypeSyntaxBuilder());

			//assert
			Assert.IsTrue(scriptBuilder.Compile().Contains(" : " + typeof(EngineScriptComponent).FullName));
			Assert.Throws<InvalidOperationException>(() => scriptBuilder.AddBaseClass<EngineScriptComponent>(new DefaultTypeSyntaxBuilder()));
			Assert.DoesNotThrow(() => scriptBuilder.AddBaseClass<ICloneable>(new DefaultTypeSyntaxBuilder()));
			Assert.IsTrue(scriptBuilder.Compile().Contains(", " + typeof(ICloneable).FullName));
		}

		[Test(Author = "Andrew Blakely", Description = "Tests the Rosyln compilation adding of a method with TestityClassBuilder.", TestOf = typeof(TestityClassBuilder))]
		public static void Test_TestityClassBuilder_Test_Adding_Method()
		{
			//arrange
			TestityClassBuilder scriptBuilder = new TestityClassBuilder(typeof(EngineScriptComponent).Name + "Script");

			Mock<IMemberImplementationProvider> implementationProvider = BuildMemberImplementationMock("TestMethod", typeof(string), MemberImplementationModifier.Public, Enumerable.Empty<Type>());

			//act
			scriptBuilder.AddMemberMethod(implementationProvider.Object, BuildBodyProviderMockEmpty().Object,
				BuildParameterProviderMock(typeof(string), typeof(int)).Object);

			//assert
			Assert.IsTrue(scriptBuilder.Compile().Contains("TestMethod(System.String String1"));
		}

		private static Mock<IMemberImplementationProvider> BuildMemberImplementationMock(string memberName, Type memberType, MemberImplementationModifier modifiers, IEnumerable<Type> attributeTypes)
		{
			Mock<IMemberImplementationProvider> implementationProvider = new Mock<IMemberImplementationProvider>();

			//Setup the implementationProvider
			implementationProvider.SetupGet(x => x.MemberName).Returns(SyntaxFactory.Identifier(memberName));
			implementationProvider.SetupGet(x => x.Type).Returns(SyntaxFactory.ParseTypeName(memberType.FullName));
			implementationProvider.SetupGet(x => x.Modifiers).Returns(SyntaxFactory.TokenList(modifiers.ToSyntaxKind().Select(x => SyntaxFactory.Token(x))));

			implementationProvider.SetupGet(x => x.Attributes)
				.Returns(SyntaxFactory.List(attributeTypes.Select(x => SyntaxFactory.AttributeList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Attribute(SyntaxFactory.IdentifierName(x.FullName)))))));

			return implementationProvider;
        }

		private static Mock<IBlockBodyProvider> BuildBodyProviderMockEmpty()
		{
			Mock<IBlockBodyProvider> bodyProvider = new Mock<IBlockBodyProvider>();

			//Empty block
			bodyProvider.SetupGet(x => x.Block).Returns(SyntaxFactory.Block());

			return bodyProvider;
		}

		private static Mock<IParameterImplementationProvider> BuildParameterProviderMock(params Type[] parameters)
		{
			Mock<IParameterImplementationProvider> parameterProvider = new Mock<IParameterImplementationProvider>();

			int paramNum = 0;

			//This is ugly but it builds a collection of parameters for the method or whatever
			ParameterListSyntax roslynParams = SyntaxFactory.ParameterList().AddParameters(
  				parameters.Select(x =>
  					SyntaxFactory.Parameter(SyntaxFactory.ParseToken(x.Name + ++paramNum))
  						.WithType(SyntaxFactory.ParseTypeName(x.FullName))
  				).ToArray());

			parameterProvider.SetupGet(x => x.Parameters).Returns(roslynParams);

			return parameterProvider;
		}
	}
}
