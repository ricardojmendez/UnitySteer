using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Testity.EngineMath
{
	public static class UnityVectorExtensions
	{
		public static UnityEngine.Vector3 ToUnityVector(this Vector3<float> vec3)
		{
			return new UnityEngine.Vector3(vec3.x, vec3.y, vec3.z);
		}
	}
}
