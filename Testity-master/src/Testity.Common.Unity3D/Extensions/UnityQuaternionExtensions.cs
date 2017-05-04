using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Testity.EngineMath
{
	public static class UnityQuaternionExtensions
	{
		public static UnityEngine.Quaternion ToUnityQuat(this Quaternion<float> quat)
		{
			return new UnityEngine.Quaternion(quat.x, quat.y, quat.z, quat.w);
		}
	}
}
