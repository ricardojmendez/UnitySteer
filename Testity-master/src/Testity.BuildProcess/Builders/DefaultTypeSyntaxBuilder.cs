using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;

namespace Testity.BuildProcess
{
	public class DefaultTypeSyntaxBuilder : ITypeSyntaxBuilder
	{
		public IEnumerable<TypeSyntax> GenerateFrom(IEnumerable<Type> types)
		{
			List<TypeSyntax> typeSyntax = new List<TypeSyntax>(types.Count());

			foreach (Type t in types)
				typeSyntax.Add(GenerateFrom(t));

			return typeSyntax;
		}

		public TypeSyntax GenerateFrom(Type t)
		{
			if (t.IsGenericType)
			{
				Type genericTypeDef = t.GetGenericTypeDefinition();

				//Get the type args
				var genericTypeList = SyntaxFactory.TypeArgumentList()
					.AddArguments(GenerateFrom(t.GetGenericArguments()).ToArray());

				return SyntaxFactory.GenericName(genericTypeDef.FullName.Remove(genericTypeDef.FullName.IndexOf('`')))
					.WithTypeArgumentList(genericTypeList);
			}
			else
				//this is fine if it's a regular type.
				return SyntaxFactory.ParseTypeName(t.FullName.Replace('+', '.')); //CodeDOM just removes +. Saves a lot of dev time http://labs.developerfusion.co.uk/SourceViewer/browse.aspx?assembly=SSCLI&namespace=Microsoft.CSharp&type=CSharpCodeGenerator
		}

		public TypeSyntax GenerateFrom<TType>()
		{
			return GenerateFrom(typeof(TType));
		}

	}
}
