using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Testity.Common
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class ExposeDataMemeberAttribute : Attribute
	{
		public ExposeDataMemeberAttribute()
		{
			//nothing
		}
	}
}
