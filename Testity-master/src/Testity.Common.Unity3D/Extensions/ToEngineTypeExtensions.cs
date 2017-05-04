using MiscUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Testity.EngineMath;
using UnityEngine;
using UnityEngine.Events;


//cant have a namespace. Must work anywhere the assembly is referenced. Required in code generation.
public static class ToEngineTypeExtensions
{
	public static Vector3<T> ToEngineType<T>(this Vector3 unityVec)
		where T : struct, IComparable<T>, IEquatable<T>
	{
		//use Jon Skeet's MiscUtil to efficiently convert T to float and return the new Unity vector
		return new Vector3<T>(Operator.Convert<float, T>(unityVec.x), Operator.Convert<float, T>(unityVec.y), Operator.Convert<float, T>(unityVec.z));
    }

	public static Vector2<T> ToEngineType<T>(this Vector2 unityVec)
	where T : struct, IComparable<T>, IEquatable<T>
	{
		//use Jon Skeet's MiscUtil to efficiently convert T to float and return the new Unity vector
		return new Vector2<T>(Operator.Convert<float, T>(unityVec.x), Operator.Convert<float, T>(unityVec.y));
	}

	#region UnityEvent and various generic counterparts
	public static Action ToEngineType(this UnityEvent unityEvent)
	{
		//if it's not null we setup the Action to call the UnityEvent
		if (unityEvent != null)
			return new Action(() => unityEvent.Invoke());
		else
			return null; //otherwise we give it an empty action
	}

	public static Action<T> ToEngineType<T>(this UnityEvent<T> unityEvent)
	{
		//if it's not null we setup the Action to call the UnityEvent
		if (unityEvent != null)
			return new Action<T>(x => unityEvent.Invoke(x));
		else
			return null; //otherwise we give it an empty action
	}

	public static Action<T1, T2> ToEngineType<T1, T2>(this UnityEvent<T1, T2> unityEvent)
	{
		//if it's not null we setup the Action to call the UnityEvent
		if (unityEvent != null)
			return new Action<T1, T2>((x, y) => unityEvent.Invoke(x, y));
		else
			return null; //otherwise we give it an empty action
	}

	public static Action<T1, T2, T3> ToEngineType<T1, T2, T3>(this UnityEvent<T1, T2, T3> unityEvent)
	{
		//if it's not null we setup the Action to call the UnityEvent
		if (unityEvent != null)
			return new Action<T1, T2, T3>((x, y, z) => unityEvent.Invoke(x, y, z));
		else
			return null; //otherwise we give it an empty action
	}

	public static Action<T1, T2, T3, T4> ToEngineType<T1, T2, T3, T4>(this UnityEvent<T1, T2, T3, T4> unityEvent)
	{
		//if it's not null we setup the Action to call the UnityEvent
		if (unityEvent != null)
			return new Action<T1, T2, T3, T4>((x, y, z, w) => unityEvent.Invoke(x, y, z, w));
		else
			return null; //otherwise we give it an empty action
	}
	#endregion

}

