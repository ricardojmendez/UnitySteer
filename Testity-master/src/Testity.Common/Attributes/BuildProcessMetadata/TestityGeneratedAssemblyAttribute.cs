using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Testity.Common
{
	/// <summary>
	/// Indicates/Marks an assembly was generated and compiled via the Testity build process.
	/// </summary>
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
	public class TestityGeneratedAssemblyAttribute : Attribute
	{

	}
}
