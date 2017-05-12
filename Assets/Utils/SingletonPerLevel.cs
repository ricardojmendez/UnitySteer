using System;
using UnityEngine;

public static class SingletonPerLevel<T> where T : Component
{
	static volatile T _instance;
	static object _lock = new object();
	
	static SingletonPerLevel()
	{
	}
	
	public static T Instance
	{
		get
		{
			if (!typeof(T).IsSubclassOf(typeof(Component))) return null;
			if (!(_instance is Component))
			{
				lock(_lock)
				{
					_instance = GameObject.FindObjectOfType(typeof(T)) as T;
					if(_instance == null && Application.isPlaying)
					{
#if TRACE_INITIALIZATION
						Debug.Log("Initializing in "+Application.loadedLevelName);
						var st = new System.Diagnostics.StackTrace(true);
			            for(int i =0; i< st.FrameCount; i++ )
			            {
			                // Note that high up the call stack, there is only
			                // one stack frame.
			                var sf = st.GetFrame(i);
			                Debug.Log(string.Format("High up the call stack, Method: {0} Line Number: {1} File: {2}",
			                    sf.GetMethod(), sf.GetFileLineNumber(), sf.GetFileName()));
			            }						
#endif

						GameObject go = new GameObject(string.Format("Singleton/Level: {0}", typeof(T).Name));
						_instance = go.AddComponent<T>();
					}
				}
			}
			return _instance;
		}
	}
	
	public static void Reset()
	{
		_instance = null;
	}
	
}
