using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Formatting;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Options;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testity.EngineComponents;

namespace Testity.BuildProcess
{
	/*public static class TestityClassBuilder
	{
		public static IClassBuilder Create(Type t)
		{
			return Activator.CreateInstance(typeof(TestityClassBuilder<>).MakeGenericType(t)) as IClassBuilder;
		}

		public static IClassBuilder Create(Type t, MemberImplementationModifier modifiers)
		{
			return Activator.CreateInstance(typeof(TestityClassBuilder<>).MakeGenericType(t), modifiers) as IClassBuilder;
		}
	}*/

	public class TestityClassBuilder : IClassBuilder
	{
		private readonly object syncObj = new object();

		private CompilationUnitSyntax rosylnCompilationUnit;

		private ClassDeclarationSyntax rosylnClassUnit;

		private IList<MemberDeclarationSyntax> memberSyntax;

		private bool hasBaseclass = false;	

		public TestityClassBuilder(string className, MemberImplementationModifier modifiers)
		{
			rosylnCompilationUnit = SyntaxFactory.CompilationUnit();
			rosylnClassUnit = SyntaxFactory.ClassDeclaration(className)
				.WithModifiers(SyntaxFactory.TokenList(modifiers.ToSyntaxKind().Select(x => SyntaxFactory.Token(x))));

			memberSyntax = new List<MemberDeclarationSyntax>();
		}

		public TestityClassBuilder(string className)
		{
			rosylnCompilationUnit = SyntaxFactory.CompilationUnit();
			rosylnClassUnit = SyntaxFactory.ClassDeclaration(className);

			memberSyntax = new List<MemberDeclarationSyntax>();
		}

		public void AddBaseClass<TClassType>(ITypeSyntaxBuilder builder)
			where TClassType : class
		{
			AddBaseClass(builder, typeof(TClassType));
        }

		public void AddBaseClass(ITypeSyntaxBuilder builder, Type classType)
		{
			if (!classType.IsClass && !classType.IsInterface)
				throw new InvalidOperationException(classType.ToString() + " is not a valid class type or interface.");		

			lock (syncObj)
			{
				//Check if there is already a baseclass
				if (hasBaseclass && classType.IsClass)
					throw new InvalidOperationException("A type may only derive from a single base class.");
				else
				{
					rosylnClassUnit = rosylnClassUnit.AddBaseListTypes(SyntaxFactory.SimpleBaseType(builder.GenerateFrom(classType)));		

					hasBaseclass = hasBaseclass || classType.IsClass;
				}
			}
		}

		//TODO: Support property fields and merge duplicate code
		public void AddClassField(IMemberImplementationProvider implementationProvider)
		{
			VariableDeclarationSyntax variableSyntax = SyntaxFactory.VariableDeclaration(implementationProvider.Type)
				.AddVariables(SyntaxFactory.VariableDeclarator(implementationProvider.MemberName));

			//New field using the information above that may be private or public.
			FieldDeclarationSyntax newField = SyntaxFactory.FieldDeclaration(variableSyntax)
				.WithAttributeLists(implementationProvider.Attributes)
				.WithModifiers(implementationProvider.Modifiers);

			lock (syncObj)
				memberSyntax.Add(newField);
		}

		public void AddMemberMethod(IMemberImplementationProvider implementationProvider, IBlockBodyProvider blockProvider, IParameterImplementationProvider parametersProvider = null)// params ParameterData[] typeArgs)
		{
			if (implementationProvider == null)
				throw new ArgumentNullException(nameof(implementationProvider), "The member implementation provider must not be null.");

			if(blockProvider == null)
				throw new ArgumentNullException(nameof(blockProvider), "The member method body block provider must not be null.");

			MethodDeclarationSyntax methodSyntax = SyntaxFactory.MethodDeclaration(implementationProvider.Type, implementationProvider.MemberName)
				.WithModifiers(implementationProvider.Modifiers)
				.WithAttributeLists(implementationProvider.Attributes)
				.WithBody(blockProvider.Block);
			
			//Not all methods have parameters so we don't want to require a provider
			if (parametersProvider != null)
				methodSyntax = methodSyntax.WithParameterList(parametersProvider.Parameters);

			lock (syncObj)
				memberSyntax.Add(methodSyntax);
		}

		public override string ToString()
		{
			return Compile();
		}

		public string Compile()
		{

			CompilationUnitSyntax compileUnit = null;
            lock (syncObj)
				//don't mutate the class fields
				//We should do it without changing them
				compileUnit = rosylnCompilationUnit.AddMembers(rosylnClassUnit.AddMembers(memberSyntax.ToArray()));

			StringBuilder sb = new StringBuilder();
			using (StringWriter writer = new StringWriter(sb))
			{
				Formatter.Format(compileUnit, new AdhocWorkspace()).WriteTo(writer);
			}

			return sb.ToString();
		}

		public Task<string> CompileAsync()
		{

			CompilationUnitSyntax compileUnit = null;

			lock (syncObj)
				//don't mutate the class fields
				//We should do it without changing them
				compileUnit = rosylnCompilationUnit.AddMembers(rosylnClassUnit.AddMembers(memberSyntax.ToArray()));

				StringBuilder sb = new StringBuilder();

				return Task.Factory.StartNew(
					() =>
					{
						using (StringWriter writer = new StringWriter(sb))
						{
							Formatter.Format(compileUnit, new AdhocWorkspace()).WriteTo(writer);
							return sb.ToString();
						}
					});
		}

		public void AddParameterlessAttributeToClass<TAttributeType>() where TAttributeType : Attribute, new()
		{
			var attriList =
				SyntaxFactory.AttributeList(
					SyntaxFactory.SeparatedList<AttributeSyntax>()
						.Add(SyntaxFactory.Attribute(SyntaxFactory.IdentifierName(typeof(TAttributeType).FullName))));

			//add the attribute to the class
			lock(syncObj)
			{
				rosylnClassUnit = rosylnClassUnit
					.AddAttributeLists(attriList);
			}
        }
	}
}
