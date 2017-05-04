using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Testity.EngineServices
{
	/// <summary>
	/// Metadata marker for engine service interfaces
	/// </summary>
	[AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = false)] //don't want anything but interfaces, no reason for multiple and classes should not be marked from inheriting
	public class EngineServiceInterfaceAttribute : Attribute
	{
		//Don't need anything
	}
}
