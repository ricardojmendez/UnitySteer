//#define TRACEDETECTED

using System;
using System.Collections.Generic;
using TickedPriorityQueue;
using UnityEngine;
using UnitySteer.Attributes;

namespace UnitySteer2D.Behaviors
{
    /// <summary>
    /// Base class for radars
    /// </summary>
    /// <remarks>
    /// The base Radar class will "ping" an area using Physics.OverlapSphere, but
    /// different radars can implement their own detection styles (if for instance
    /// they wish to handle a proximity quadtre/octree themselves).
    /// 
    /// It expects that every object to be detected by the radar will report  via
    /// AddDetectableObject on enable, and remove itself via RemoveDetectableObject
    /// on disable.
    /// </remarks>
    [AddComponentMenu("UnitySteer2D/Tools/Radar")]
    public class Radar2D : MonoBehaviour
    {
        #region Private static properties

        private static IDictionary<int, DetectableObject2D> _knownDetectableObjects =
            new SortedDictionary<int, DetectableObject2D>();

        #endregion

        #region Private properties

        private Transform _transform;
        private TickedObject _tickedObject;
        private UnityTickedQueue _steeringQueue;

        [SerializeField] private string _queueName = "Radar2D";

        /// <summary>
        /// The maximum number of radar update calls processed on the queue per update
        /// </summary>
        /// <remarks>
        /// Notice that this is a limit shared across queue items of the same name, at
        /// least until we have some queue settings, so whatever value is set last for 
        /// the queue will win.  Make sure your settings are consistent for objects of
        /// the same queue.
        /// </remarks>
        [SerializeField] private int _maxQueueProcessedPerUpdate = 20;

        /// <summary>
        /// How often is the radar updated
        /// </summary>
        [SerializeField] private float _tickLength = 0.5f;

        [SerializeField] private float _detectionRadius = 5;

        [SerializeField] private bool _detectDisabledVehicles;

        [SerializeField] private LayerMask _layersChecked;

        [SerializeField] private bool _drawGizmos;

        [SerializeField] private int _preAllocateSize = 30;

        private Collider2D[] _detectedColliders;
        private List<DetectableObject2D> _detectedObjects;
        private List<Vehicle2D> _vehicles;
        private List<DetectableObject2D> _obstacles;

        #endregion

        #region Public properties

        /// <summary>
        /// List of currently detected neighbors
        /// </summary>
        public IEnumerable<Collider2D> Detected
        {
            get { return _detectedColliders; }
        }

        /// <summary>
        /// Radar ping detection radius
        /// </summary>
        public float DetectionRadius
        {
            get { return _detectionRadius; }
            set { _detectionRadius = value; }
        }


        /// <summary>
        /// Indicates if the radar will detect disabled vehicles. 
        /// </summary>
        public bool DetectDisabledVehicles
        {
            get { return _detectDisabledVehicles; }
            set { _detectDisabledVehicles = value; }
        }

        /// <summary>
        /// Should the radar draw its gizmos?
        /// </summary>
        public bool DrawGizmos
        {
            get { return _drawGizmos; }
            set { _drawGizmos = value; }
        }

        /// <summary>
        /// List of obstacles detected by the radar
        /// </summary>
        public List<DetectableObject2D> Obstacles
        {
            get { return _obstacles; }
        }

        /// <summary>
        /// Returns the radars position
        /// </summary>
        public Vector2 Position
        {
            get { return (Vehicle != null) ? Vehicle.Position : (Vector2)_transform.position; }
        }

        public Action<Radar2D> OnDetected = delegate { };

        /// <summary>
        /// Gets the vehicle this radar is attached to
        /// </summary>
        public Vehicle2D Vehicle { get; private set; }

        /// <summary>
        /// List of vehicles detected among the colliders
        /// </summary>
        public List<Vehicle2D> Vehicles
        {
            get { return _vehicles; }
        }

        /// <summary>
        /// Layer mask for the object layers checked
        /// </summary>
        public LayerMask LayersChecked
        {
            get { return _layersChecked; }
            set { _layersChecked = value; }
        }

#endregion

#region Static methods

        /// <summary>
        /// Must be called when a detectable object is enabled so they can be easily identified
        /// </summary>
        /// <param name="obj">Detectable object</param>
        public static void AddDetectableObject2D(DetectableObject2D obj)
        {
            _knownDetectableObjects[obj.Collider.GetInstanceID()] = obj;
        }

        /// <summary>
        /// Must be called when a detectable object is disabled to remove it from the list of known objects
        /// </summary>
        /// <param name="obj">Detectable object</param>
        /// <returns>True if the call to Remove succeeded</returns>
        public static bool RemoveDetectableObject2D(DetectableObject2D obj)
        {
            return _knownDetectableObjects.Remove(obj.Collider.GetInstanceID());
        }

#endregion

#region Methods

        protected virtual void Awake()
        {
            Vehicle = GetComponent<Vehicle2D>();
            _transform = transform;
            _vehicles = new List<Vehicle2D>(_preAllocateSize);
            _obstacles = new List<DetectableObject2D>(_preAllocateSize);
            _detectedObjects = new List<DetectableObject2D>(_preAllocateSize * 3);
        }


        private void OnEnable()
        {
            _tickedObject = new TickedObject(OnUpdateRadar) {TickLength = _tickLength};
            _steeringQueue = UnityTickedQueue.GetInstance(_queueName);
            _steeringQueue.Add(_tickedObject);
            _steeringQueue.MaxProcessedPerUpdate = _maxQueueProcessedPerUpdate;
        }


        private void OnDisable()
        {
            if (_steeringQueue != null)
            {
                _steeringQueue.Remove(_tickedObject);
            }
        }


        private void OnUpdateRadar(object obj)
        {
            UnityEngine.Profiling.Profiler.BeginSample("OnUpdateRadar");
            _detectedColliders = Detect();
            FilterDetected();
            if (OnDetected != null)
            {
                UnityEngine.Profiling.Profiler.BeginSample("Detection event handler");
                OnDetected(this);
                UnityEngine.Profiling.Profiler.EndSample();
            }
#if TRACEDETECTED
		if (DrawGizmos)
		{
			Debug.Log(gameObject.name+" detected at "+Time.time);
			var sb = new System.Text.StringBuilder(); 
			foreach (var v in Vehicles)
			{
				sb.Append(v.gameObject.name);
				sb.Append(" ");
				sb.Append(v.Position);
				sb.Append(" ");
			}
			foreach (var o in Obstacles)
			{
				sb.Append(o.gameObject.name);
				sb.Append(" ");
				sb.Append(o.Position);
				sb.Append(" ");
			}
			Debug.Log(sb.ToString());
		}
#endif
            UnityEngine.Profiling.Profiler.EndSample();
        }

        public void UpdateRadar()
        {
            OnUpdateRadar(null);
        }

        protected virtual Collider2D[] Detect()
        {
            return Physics2D.OverlapCircleAll(Position, DetectionRadius, LayersChecked);
        }

        protected virtual void FilterDetected()
        {
            /*
		 * For each detected item, obtain the DetectableObject it has.
		 * We could have allowed people to have multiple DetectableObjects 
		 * on a transform hierarchy, but this ends up with us having to do
		 * calls to GetComponentsInChildren, which gets really expensive.
		 * 
		 * I *do not* recommend changing this to GetComponentsInChildren.
		 * As a reference, whenever the radar fired up near a complex object
		 * (say, a character model) obtaining the list of DetectableObjects
		 * took about 75% of the time used for the frame.
         * 
		 */
            UnityEngine.Profiling.Profiler.BeginSample("Base FilterDetected");

            _vehicles.Clear();
            _obstacles.Clear();
            _detectedObjects.Clear();


            UnityEngine.Profiling.Profiler.BeginSample("Initial detection");
            for (var i = 0; i < _detectedColliders.Length; i++)
            {
                var id = _detectedColliders[i].GetInstanceID();
                if (!_knownDetectableObjects.ContainsKey(id))
                    continue; // Ignore anything that hadn't previously registered as a detectable object
                var detectable = _knownDetectableObjects[id];
                // It's possible that d != null but that d.Equals(null) if the
                // game object has been marked as destroyed by Unity between
                // detection and filtering.
                if (detectable != null &&
                    detectable != Vehicle &&
                    !detectable.Equals(null))
                {
                    _detectedObjects.Add(detectable);
                }
            }
            UnityEngine.Profiling.Profiler.EndSample();

            UnityEngine.Profiling.Profiler.BeginSample("Filtering out vehicles");
            for (var i = 0; i < _detectedObjects.Count; i++)
            {
                var d = _detectedObjects[i];
                var v = d as Vehicle2D;
                if (v != null && (v.enabled || _detectDisabledVehicles))
                {
                    _vehicles.Add(v);
                }
                else
                {
                    _obstacles.Add(d);
                }
            }
            UnityEngine.Profiling.Profiler.EndSample();
            UnityEngine.Profiling.Profiler.EndSample();
        }
#endregion

#region Unity methods

        private void OnDrawGizmos()
        {
            if (_drawGizmos)
            {
                var pos = (Vehicle == null) ? (Vector2)transform.position : Vehicle.Position;

                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(pos, DetectionRadius);
#if TRACEDETECTED
			if (Application.isPlaying) {
				foreach (var v in Vehicles)
					Gizmos.DrawLine(pos, v.gameObject.transform.position);
				foreach (var o in Obstacles)
					Gizmos.DrawLine(pos, o.gameObject.transform.position);
			}
#endif
            }
        }

#endregion
    }
}