using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Testity.Common
{
	/// <summary>
	/// Marks the field that is the game logic class instance inside of potential wrapper classes.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
	public class ImplementationField : ImplementationMetadataAttribute
	{
		private readonly string FieldName;

		public ImplementationField(EngineType type, string fieldName)
			: base(type)
		{
			FieldName = fieldName;
		}
	}
}
