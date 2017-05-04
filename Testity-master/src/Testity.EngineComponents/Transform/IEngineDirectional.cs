using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Testity.Common;
using Testity.EngineMath;

namespace Testity.EngineComponents
{
	/// <summary>
	/// Represents and provides simple directions.
	/// Not based on anything in Unity3D; maybe the Transform.back and etc.
	/// </summary>
	[EngineComponentInterface]
	[EngineSerializableMapsToType(EngineType.Unity3D, "UnityEngine.Transform, UnityEngine")]
	public interface IEngineDirectional
	{
		/// <summary>
		/// Represents a forward vector.
		/// </summary>
		Vector3<float> Forward { get; }

		/// <summary>
		/// Represents a back vector.
		/// </summary>
		Vector3<float> Back { get; }

		/// <summary>
		/// Represents a up vector.
		/// </summary>
		Vector3<float> Up { get; }

		/// <summary>
		/// Represents a down vector.
		/// </summary>
		Vector3<float> Down { get; }

		/// <summary>
		/// Represents a right vector.
		/// </summary>
		Vector3<float> Right { get; }

		/// <summary>
		/// Represents a left vector.
		/// </summary>
		Vector3<float> Left { get; }
	}
}
