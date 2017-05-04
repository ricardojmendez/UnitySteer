using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Testity.EngineServices.Unity3D
{
	/// <summary>
	/// Adapter that adapts Unity3D's <see cref="UnityEngine.Time"/> class to the <see cref="IEngineTimeService"/> interface.
	/// </summary>
	[EngineServiceConcrete]
	public class UnityEngineTimeServiceAdapter : IEngineTimeService
	{
		/// <summary>
		/// Total time passed since the begining of a scene unit in seconds.
		/// This may not be accurate to the time passed since the begining of the current frame.
		/// Is NOT modified by <see cref="GlobalTimeScale"/>.
		/// WARNING: May be platform dependent and unstable.
		/// Based on Unity3D's: http://docs.unity3d.com/ScriptReference/Time-unscaledTime.html.
		/// Based on UnrealEngine 4's: https://docs.unrealengine.com/latest/INT/API/Runtime/Engine/Engine/UWorld/GetRealTimeSeconds/index.html
		/// </summary>
		public float AbsoluteTimePassed { get { return Time.unscaledTime; } }

		/// <summary>
		/// Time passed since last fixed frame update in seconds.
		/// Based on Unity3D's: http://docs.unity3d.com/ScriptReference/Time-fixedDeltaTime.html
		/// Based on UnrealEngine 4's: https://docs.unrealengine.com/latest/INT/API/Runtime/Core/Misc/FApp/GetFixedDeltaTime/index.html (best I could find)
		/// </summary>
		public float FixedDeltaTime { get { return Time.fixedDeltaTime; } }

		/// <summary>
		/// Indicates the state of <see cref="FixedDeltaTime"/>'s computation. If false then <see cref="FixedDeltaTime"/> may not report correct timing.
		/// Based on Unity3D's: No known implementation. Included for UnrealEngine 4's sake.
		/// Based on UnrealEngine 4's: https://docs.unrealengine.com/latest/INT/API/Runtime/Core/Misc/FApp/UseFixedTimeStep/index.html
		/// </summary>
		public bool FixedTimeStepEnabled { get { return true; } } //always enabled in Unity

		/// <summary>
		/// Time passed since last frame in seconds.
		/// Generally depends on <see cref="GlobalTimeScale"/>
		/// Based on Unity3D's: http://docs.unity3d.com/ScriptReference/Time-deltaTime.html
		/// Based on UnrealEngine 4's: https://docs.unrealengine.com/latest/INT/API/Runtime/Engine/Engine/UWorld/GetDeltaSeconds/index.html
		/// </summary>
		public float FrameDeltaTime { get { return Time.deltaTime; } }

		/// <summary>
		/// Scale at which time passes. Essentially this is a coefficient applied to all non absolute time values.
		/// Default value being 1.0f which is normal rate of time.
		/// Based on Unity3D's: http://docs.unity3d.com/ScriptReference/Time-timeScale.html
		/// Based on UnrealEngine 4's: https://docs.unrealengine.com/latest/INT/API/Runtime/Engine/Kismet/UGameplayStatics/GetGlobalTimeDilation/index.html (maybe)
		/// Additional Notes: (Some engines provide GameObject/Actor/Entity specific dilation/timescale)
		/// </summary>
		public float GlobalTimeScale { get { return Time.timeScale; } set { Time.timeScale = value; } }

		/// <summary>
		/// Total time passed since the begining of a scene unit in seconds.
		/// This may not be accurate to the time passed since the begining of the current frame.
		/// Is modified by <see cref="GlobalTimeScale"/>.
		/// Based on Unity3D's: http://docs.unity3d.com/ScriptReference/Time-time.html
		/// Based on UnrealEngine 4's: https://docs.unrealengine.com/latest/INT/API/Runtime/Engine/Engine/UWorld/GetTimeSeconds/index.html
		/// </summary>
		public float TimePassed { get { return Time.time; } }
	}
}
