using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Testity.Common;

namespace Testity.EngineComponents
{
	[Serializable]
	[EngineSerializableMapsToType(EngineType.Unity3D, "UnityEngine.MonoBehaviour, UnityEngine")]
	public abstract class EngineScriptComponent : IEngineComponent, IEquatable<EngineScriptComponent>
	{
		/// <summary>
		/// Defaults to name of Type. Can be set.
		/// </summary>
		public string Name { get; set; }

		public EngineScriptComponent()
		{
			Name = GetType().ToString();
		}

		public virtual void Dispose()
		{
			//do nothing
		}

		public bool Equals(IEngineObject other)
		{
			return this == other;
		}

		public bool Equals(EngineScriptComponent other)
		{
			return other == this;
		}

		public static bool operator ==(EngineScriptComponent lhs, EngineScriptComponent rhs)
		{
			return System.Object.ReferenceEquals(lhs, rhs);
		}

		public static bool operator !=(EngineScriptComponent lhs, EngineScriptComponent rhs)
		{
			return !System.Object.ReferenceEquals(lhs, rhs);
		}

		public static bool operator ==(EngineScriptComponent lhs, IEngineObject rhs)
		{
			return System.Object.ReferenceEquals(lhs, rhs);
		}

		public static bool operator !=(EngineScriptComponent lhs, IEngineObject rhs)
		{
			return !System.Object.ReferenceEquals(lhs, rhs);
		}

		public static bool operator ==(IEngineObject lhs, EngineScriptComponent rhs)
		{
			return System.Object.ReferenceEquals(lhs, rhs);
		}

		public static bool operator !=(IEngineObject lhs, EngineScriptComponent rhs)
		{
			return !System.Object.ReferenceEquals(lhs, rhs);
		}
	}
}
