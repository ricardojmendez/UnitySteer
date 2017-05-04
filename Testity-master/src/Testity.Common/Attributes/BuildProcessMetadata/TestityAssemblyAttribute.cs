using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Testity.Common
{
	/// <summary>
	/// Indicates/Marks an assembly that indicates to consuming engines that this is a special assembly for Testity
	/// This should indicate that this assembly requires post processing after import.
	/// </summary>
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
	public class TestityAssemblyAttribute : Attribute
	{

	}
}
