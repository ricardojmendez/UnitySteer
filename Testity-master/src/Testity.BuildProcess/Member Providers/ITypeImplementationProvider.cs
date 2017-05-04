using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testity.BuildProcess
{
	public interface ITypeImplementationProvider
	{
		TypeSyntax Type { get; }
	}
}
