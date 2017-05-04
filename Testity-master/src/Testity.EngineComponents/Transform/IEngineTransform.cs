using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Testity.Common;
using Testity.EngineMath;

namespace Testity.EngineComponents
{
	/// <summary>
	/// A transformation data interface.
	/// Based on Unity3D's: http://docs.unity3d.com/ScriptReference/Transform.html
	/// </summary>
	[EngineComponentInterface]
	[EngineSerializableMapsToType(EngineType.Unity3D, "UnityEngine.Transform, UnityEngine")]
	public interface IEngineTransform : IEngineComponent, IEngineDirectional
	{
		/// <summary>
		/// Position of the transform in world space.
		/// Based on Unity3D's: http://docs.unity3d.com/ScriptReference/Transform-position.html
		/// </summary>
		Vector3<float> Position { get; set; }

		/// <summary>
		/// Position of the transform relative it's parent's transform. Just <see cref="Position"/> if there is no parent.
		/// Based on Unity3D's: http://docs.unity3d.com/ScriptReference/Transform-localPosition.html
		/// </summary>
		Vector3<float> LocalPosition { get; set; }

		/// <summary>
		/// Position of the transform relative to its parent
		/// Based on Unity3D's: http://docs.unity3d.com/ScriptReference/Transform-localScale.html
		/// </summary>
		Vector3<float> LocalScale { get; set; }

		/// <summary>
		/// Rotation of the transform in world space.
		/// Based on Unity3D's: http://docs.unity3d.com/ScriptReference/Transform-rotation.html
		/// </summary>
		Quaternion<float> Rotation { get; set; }

		/// <summary>
		/// Rotation of the transform relative to a parent's transform. Just <see cref="Rotation"/> if there is no parent.
		/// Based on Unity3D's: http://docs.unity3d.com/ScriptReference/Transform-localRotation.html
		/// </summary>
		Quaternion<float> LocalRotation { get; set; }
	}
}
