using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testity.BuildProcess.Unity3D
{
	public class ClassFile
	{
		public readonly string ClassData;

		public readonly string ClassName;

		public ClassFile(string data, string name)
		{
			ClassData = data;
			ClassName = name;
		}
	}
}
