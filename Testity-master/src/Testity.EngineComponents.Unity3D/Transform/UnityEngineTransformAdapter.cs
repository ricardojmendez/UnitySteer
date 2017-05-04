using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Testity.Common;
using Testity.EngineMath;
using UnityEngine;

namespace Testity.EngineComponents.Unity3D
{
	[Serializable]
	[EngineComponentConcrete(typeof(IEngineTransform), typeof(IEngineDirectional))]
	[EngineComponentAdapter(typeof(UnityEngine.Transform))]
	public class UnityEngineTransformAdapter : IEngineTransform, IEngineDirectional
	{
		public Vector3<float> Back
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public Vector3<float> Down
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public Vector3<float> Forward
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public Vector3<float> Left
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public Vector3<float> LocalPosition
		{
			get
			{
				throw new NotImplementedException();
			}

			set
			{
				throw new NotImplementedException();
			}
		}

		public Quaternion<float> LocalRotation
		{
			get
			{
				throw new NotImplementedException();
			}

			set
			{
				throw new NotImplementedException();
			}
		}

		public Vector3<float> LocalScale
		{
			get
			{
				throw new NotImplementedException();
			}

			set
			{
				throw new NotImplementedException();
			}
		}

		public Vector3<float> Position
		{
			get
			{
				throw new NotImplementedException();
			}

			set
			{
				throw new NotImplementedException();
			}
		}

		public Vector3<float> Right
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public Quaternion<float> Rotation
		{
			get
			{
				throw new NotImplementedException();
			}

			set
			{
				throw new NotImplementedException();
			}
		}

		public Vector3<float> Up
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public string Name
		{
			get
			{
				throw new NotImplementedException();
			}

			set
			{
				throw new NotImplementedException();
			}
		}

		private readonly UnityEngine.Transform transformAdaptee;

		[EngineComponentAdapterConstructor(typeof(UnityEngine.Transform))]
		public UnityEngineTransformAdapter(UnityEngine.Transform transform)
		{
			transformAdaptee = transform;
		}

		public bool Equals(IEngineObject other)
		{
			return this == other;
		}

		public static bool operator ==(UnityEngineTransformAdapter lhs, UnityEngineTransformAdapter rhs)
		{
			return System.Object.ReferenceEquals(lhs, rhs);
		}

		public static bool operator !=(UnityEngineTransformAdapter lhs, UnityEngineTransformAdapter rhs)
		{
			return !System.Object.ReferenceEquals(lhs, rhs);
		}

		public static bool operator ==(UnityEngineTransformAdapter lhs, IEngineObject rhs)
		{
			return System.Object.ReferenceEquals(lhs, rhs);
		}

		public static bool operator !=(UnityEngineTransformAdapter lhs, IEngineObject rhs)
		{
			return !System.Object.ReferenceEquals(lhs, rhs);
		}

		public static bool operator ==(IEngineObject lhs, UnityEngineTransformAdapter rhs)
		{
			return System.Object.ReferenceEquals(lhs, rhs);
		}

		public static bool operator !=(IEngineObject lhs, UnityEngineTransformAdapter rhs)
		{
			return !System.Object.ReferenceEquals(lhs, rhs);
		}

		public void Dispose()
		{
			throw new NotImplementedException();
		}
	}
}
