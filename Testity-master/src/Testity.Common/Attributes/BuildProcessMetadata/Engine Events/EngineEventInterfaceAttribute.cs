using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Testity.EngineComponents
{
	/// <summary>
	/// Metadata market for interfaces that handle engine events.
	/// </summary>
	[AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
	public class EngineEventInterfaceAttribute : Attribute
	{

	}
}
