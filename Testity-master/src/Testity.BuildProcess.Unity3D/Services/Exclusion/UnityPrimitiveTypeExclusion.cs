using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testity.BuildProcess.Unity3D
{
	public class UnityPrimitiveTypeExclusion : IEnumerable<Type>, ITypeExclusion
	{
		private readonly Type[] excludedPrimitiveTypes
			= new Type[] { typeof(IntPtr), typeof(UIntPtr) };

		public UnityPrimitiveTypeExclusion()
		{
			//do nothing
		}

		public IEnumerator<Type> GetEnumerator()
		{
			return excludedPrimitiveTypes.AsEnumerable<Type>().GetEnumerator();
        }

		public bool isExcluded(Type t)
		{
			return excludedPrimitiveTypes.Contains(t);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return excludedPrimitiveTypes.GetEnumerator();
        }
	}
}
