using TickedPriorityQueue;
using UnityEngine;

namespace UnitySteer.Tools
{
    [AddComponentMenu("UnitySteer/Vehicle/Speedometer")]
    public class Speedometer : MonoBehaviour
    {
        private Vector3 _lastRecordedPosition;

        /// <summary>
        /// Array of the measured squared magnitudes of the position deltas between measures
        /// </summary>
        private float[] _squaredDistanceSamples;

        private float _cachedSpeed;
        private float _lastAverageCalculationTime;


        private Transform _transform;

        private TickedObject _tickedObject;
        private UnityTickedQueue _queue;

        [SerializeField] private string _queueName = "Steering";

        /// <summary>
        /// Where did we last record a speed? The speed recording method will
        /// go  around the array in a loop.
        /// </summary>
        private int _lastSampleIndex;


        /// <summary>
        /// How often is the average speed refreshed if requested by the user
        /// </summary>
        [SerializeField] private float _cachedSpeedRefreshRate = 1f;


        /// <summary>
        /// How often is a position sample taken. Private, so that it is not
        /// updated at runtime.
        /// </summary>
        [SerializeField] private float _measuringSpeed = 0.25f;

        /// <summary>
        /// Total number of samples that we should keep around. Private, so 
        /// that it is not updated at runtime.
        /// </summary>
        [SerializeField] private int _numberSamples = 10;


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
                    for (var i = 0; i < _numberSamples; i++)
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


        private void Awake()
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


        private void OnMeasureSpeed(object param)
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