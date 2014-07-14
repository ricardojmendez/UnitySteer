using UnityEngine;
using TickedPriorityQueue;

namespace UnitySteer.Tools
{

[AddComponentMenu("UnitySteer/Vehicle/Speedometer")]
public class Speedometer : MonoBehaviour 
{
	Vector3 _lastRecordedPosition;
	
	/// <summary>
	/// Array of the measured squared magnitudes of the position deltas between measures
	/// </summary>
	float[] _squaredDistanceSamples;
	
	float _cachedSpeed = 0;
	float _lastAverageCalculationTime = 0;
	
	
	Transform _transform;
	
	TickedObject _tickedObject;
	UnityTickedQueue _queue;
	
	[SerializeField]
	string _queueName = "Steering";
	
	/// <summary>
	/// Where did we last record a speed? The speed recording method will
	/// go  around the array in a loop.
	/// </summary>
	int _lastSampleIndex = 0;
	
	
	/// <summary>
	/// How often is the average speed refreshed if requested by the user
	/// </summary>
	[SerializeField]
	float _cachedSpeedRefreshRate = 1f;
	
	
	/// <summary>
	/// How often is a position sample taken. Private, so that it is not
	/// updated at runtime.
	/// </summary>
	[SerializeField]
	float _measuringSpeed = 0.25f;
	
	/// <summary>
	/// Total number of samples that we should keep around. Private, so 
	/// that it is not updated at runtime.
	/// </summary>
	[SerializeField]
	int _numberSamples = 10;
	
	
	/// <summary>
	/// Current speed
	/// </summary>
	public float Speed
	{
		get 
		{
			if (Time.time > _lastAverageCalculationTime + _cachedSpeedRefreshRate)
			{
				_lastAverageCalculationTime = Time.time;
				_cachedSpeed = 0;
				for (int i = 0; i < _numberSamples; i++)
				{
					_cachedSpeed += _squaredDistanceSamples[i];
				}
				_cachedSpeed /= _numberSamples;
				_cachedSpeed = Mathf.Sqrt(_cachedSpeed);
				_cachedSpeed /= _measuringSpeed;
			}
			return _cachedSpeed;
		}
	}
	
	
	void Awake()
	{
		_transform = transform;
		_lastRecordedPosition = _transform.position;
		_squaredDistanceSamples = new float[_numberSamples];
	}
	
	protected void OnEnable()
	{
		// Initialize the behavior tree and its queue
		_tickedObject = new TickedObject(OnMeasureSpeed);
		_tickedObject.TickLength = _measuringSpeed;
		_queue = UnityTickedQueue.GetInstance(_queueName);
		_queue.Add(_tickedObject);
	}
	
	protected void OnDisable()
	{
		if (_queue != null)
		{
			_queue.Remove(_tickedObject);
		}
	}
	
	
	void OnMeasureSpeed(object param)
	{
		// Cycle the samples
		if (++_lastSampleIndex >= _numberSamples)
		{
			_lastSampleIndex = 0;
		}
		_squaredDistanceSamples[_lastSampleIndex] = (_transform.position - _lastRecordedPosition).sqrMagnitude;
		_lastRecordedPosition = _transform.position;
	}
}

}
