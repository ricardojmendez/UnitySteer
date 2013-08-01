using System;
using UnityEngine;
using TickedPriorityQueue;
using System.Collections.Generic;

/// <summary>
/// A helper class to create Unity updated Ticked Priority Queues.
/// Instance will return the default instance, and CreateInstance will return a named instance.
/// MaxProcessedPerUpdate will get or set the max number of items to be processed per Unity update.
/// </summary>
public class UnityTickedQueue : MonoBehaviour
{
	#region Instances
	private static Dictionary<string, UnityTickedQueue> _instances;
	private static UnityTickedQueue _instance;

	/// <summary>
	/// Retrieves a default static instance for ease of use.
	/// The name of the created GameObject will be Ticked Queue.
	/// </summary>
	public static UnityTickedQueue Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = CreateInstance(null);
			}
			return _instance;
		}
	}
	
	/// <summary>
	/// Retrieves a named custom instance.
	/// The queue's GameObject will be named Ticked Queue - name.
	/// If the name already exists, it will retrieve the older named instance.
	/// </summary>
	/// <param name="name">
	/// A <see cref="System.String"/> giving the name of the queue.
	/// </param>
	/// <returns>
	/// A <see cref="UnityTickedQueue"/> instance.
	/// </returns>
	public static UnityTickedQueue GetInstance(string name)
	{
		if (string.IsNullOrEmpty(name)) return Instance;
		name = name.ToLower();
		
		UnityTickedQueue queue = null;
		if (_instances == null)
			_instances = new Dictionary<string, UnityTickedQueue>();
		else
		{
			_instances.TryGetValue(name, out queue);
		}
		
		if (queue == null)
		{
			queue = CreateInstance(name);
			_instances[name] = queue;
		}
		
		return queue;
	}
	
	private static UnityTickedQueue CreateInstance(string name)
	{
		if (string.IsNullOrEmpty(name)) name = "Ticked Queue";
		else name = "Ticked Queue - " + name;
		GameObject go = new GameObject(name);
		return go.AddComponent<UnityTickedQueue>();
	}
	#endregion
	
	private TickedQueue _queue = new TickedQueue();

	public bool IsPaused {
		get {
			return _queue.IsPaused;
		}
		set {
			_queue.IsPaused = value;

			enabled = !value;
		}
	}
		
	public TickedQueue Queue {
		get {
			return this._queue;
		}
	}
	
	
	private UnityTickedQueue () {}	
	
	/// <summary>
	/// Adds an ITicked reference to the queue.
	/// </summary>
	/// <param name="ticked">
	/// A <see cref="ITicked"/> reference, which will be ticked periodically based on its properties.
	/// </param>
	public void Add(ITicked ticked)
	{
		_queue.Add(ticked);
	}
	
	/// <summary>
	/// Removes an ITicked reference from the queue.
	/// </summary>
	/// <param name="ticked">
	/// A <see cref="ITicked"/> reference, which will be ticked periodically based on its properties.
	/// </param>
	public void Remove(ITicked ticked)
	{
		_queue.Remove(ticked);
	}
	
	/// <summary>
	/// Sets the maximum number of items to be processed every time Unity updates.
	/// </summary>
	public int MaxProcessedPerUpdate
	{
		get { return _queue.MaxProcessedPerUpdate; }
		set { _queue.MaxProcessedPerUpdate = value; }
	}
	
	private void Update()
	{
		_queue.Update();
	}
}

