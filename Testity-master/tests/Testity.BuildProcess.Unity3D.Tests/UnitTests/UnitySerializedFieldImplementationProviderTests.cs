using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testity.Common.Unity3D;
using UnityEngine;

namespace Testity.BuildProcess.Unity3D.Tests
{
	[TestFixture]
	public static class UnitySerializedFieldImplementationProviderTests
	{
		[Test]
		public static void Test_UnitySerializedFieldImplementationProvider_Attribute_Method_Generator()
		{
			//arrange
			WiredToAttribute attri = new WiredToAttribute(System.Reflection.MemberTypes.Property, "SomethingProp", typeof(string).AssemblyQualifiedName);
			UnitySerializedFieldImplementationProvider provider = new UnitySerializedFieldImplementationProvider("blah", typeof(string), attri);
			string serializeFieldName = typeof(SerializeField).FullName;
			string compiledAttributes = null;

			//act
			var compileUnit = SyntaxFactory.CompilationUnit().
				WithAttributeLists(provider.Attributes)
				.WithEndOfFileToken(SyntaxFactory.Token(SyntaxKind.EndOfFileToken));

			StringBuilder sb = new StringBuilder();

			using (StringWriter writer = new StringWriter(sb))
			{
				Formatter.Format(compileUnit, new AdhocWorkspace()).WriteTo(writer);
			}

			compiledAttributes = sb.ToString();

			//assert
			//Tests that it contains [SerializeField]
			Assert.IsTrue(compiledAttributes.Contains(@"[" + typeof(SerializeField).FullName + @"(),") || compiledAttributes.Contains(@"[" + typeof(SerializeField).FullName + @","));
			//Tests that it contains WiredToAttribute(
			Assert.IsTrue(compiledAttributes.Contains(typeof(WiredToAttribute).FullName + @"("));
			//Tests that it contains ,"NAME" or , "NAME"
			Assert.IsTrue(compiledAttributes.Contains(@",""" + attri.WiredMemberName + @"""") || compiledAttributes.Contains(@", """ + attri.WiredMemberName + @""""));

			Assert.IsTrue(compiledAttributes.Contains(@",""" + attri.TypeWiredTo.FullName) || compiledAttributes.Contains(@", """ + attri.TypeWiredTo.FullName));
		}
	}
}
