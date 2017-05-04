using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Testity.Common;

namespace Testity.EngineComponents
{
	/// <summary>
	/// Implements offers the most basic of functionality required for engine objects.
	/// </summary>
	[EngineComponentInterface]
	public interface IEngineObject : IEquatable<IEngineObject>, IDisposable
	{
		/// <summary>
		/// Get or set the the <see cref="IEngineObject"/>.
		/// Based on Unity3D's: http://docs.unity3d.com/ScriptReference/Object-name.html
		/// I think UnrealEngine4's (not sure): https://docs.unrealengine.com/latest/INT/API/Runtime/CoreUObject/UObject/UObject/Rename/index.html
		/// </summary>
		string Name { get; set; }
	}
}
