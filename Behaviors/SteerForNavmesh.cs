#define ANNOTATE_NAVMESH
using UnityEngine;

namespace UnitySteer.Behaviors
{
    /// <summary>
    /// Steers a vehicle to stay on the navmesh
    /// Currently only supports the Default layer
    /// </summary>
    [AddComponentMenu("UnitySteer/Steer/... for Navmesh")]
    public class SteerForNavmesh : Steering
    {
        #region Private fields

        [SerializeField] private float _avoidanceForceFactor = 0.75f;

        [SerializeField] private float _minTimeToCollision = 2;

        [SerializeField] private bool _offMeshCheckingEnabled = true;

        [SerializeField] private Vector3 _probePositionOffset = new Vector3(0, 0.2f, 0);

        [SerializeField] private float _probeRadius = 0.1f;

        // TODO navmesh layer selection -> CustomEditor -> GameObjectUtility.GetNavMeshLayerNames() + Popup

        #endregion

        #region Public properties

        /// <summary>
        /// Multiplier for the force applied on avoidance
        /// </summary>
        /// <remarks>If his value is set to 1, the behavior will return an
        /// avoidance force that uses the full brunt of the vehicle's maximum
        /// force.</remarks>
        public float AvoidanceForceFactor
        {
            get { return _avoidanceForceFactor; }
            set { _avoidanceForceFactor = value; }
        }

        /// <summary>
        /// Minimum time to collision to consider
        /// </summary>
        public float MinTimeToCollision
        {
            get { return _minTimeToCollision; }
            set { _minTimeToCollision = value; }
        }

        /// <summary>
        /// Switch if off-mesh checking should be done or not.
        /// </summary>
        /// <remarks>Off-mesh chekcing, checks if the Vehicle is currently on the navmesh or not.
        /// If not, a force is calculated to bring it back on it.
        /// </remarks>
        public bool OffMeshChecking
        {
            get { return _offMeshCheckingEnabled; }
            set { _offMeshCheckingEnabled = value; }
        }

        /// <summary>
        /// Offset where to place the off-mesh checking probe, relative to the Vehicle position
        /// </summary>
        /// <remarks>This should be as close to the navmesh height as possible. Normally 
        /// it's slightly floating above the ground (0.2 with default settings on a simple plain).
        /// </remarks>
        public Vector3 ProbePositionOffset
        {
            get { return _probePositionOffset; }
            set { _probePositionOffset = value; }
        }

        /// <summary>
        /// Offset where to place the off-mesh checking probe, relative to the Vehicle position
        /// </summary>
        /// <remarks>The radius makes it possible to compensate slight variations in the navmesh
        /// heigh. However, this setting  affects the horizontal tolerance as well. This means,
        /// the larger the radius, the later the vehicle will be considered off mesh.
        /// </remarks>
        public float ProbeRadius
        {
            get { return _probeRadius; }
            set { _probeRadius = value; }
        }

        #endregion

        private int _navMeshLayerMask;

        protected override void Start()
        {
            base.Start();
            _navMeshLayerMask = 1 << NavMesh.GetNavMeshLayerFromName("Default");
        }


        public override bool IsPostProcess
        {
            get { return true; }
        }

        /// <summary>
        /// Calculates the force necessary to stay on the navmesh
        /// </summary>
        /// <returns>
        /// Force necessary to stay on the navmesh, or Vector3.zero
        /// </returns>
        /// <remarks>
        /// If the Vehicle is too far off the navmesh, Vector3.zero is retured.
        /// This won't lead back to the navmesh, but there's no way to determine
        /// a way back onto it.
        /// </remarks>
        protected override Vector3 CalculateForce()
        {
            NavMeshHit hit;

            /*
		 * While we could just calculate line as (Velocity * predictionTime) 
		 * and save ourselves the substraction, this allows other vehicles to
		 * override PredictFuturePosition for their own ends.
		 */
            var futurePosition = Vehicle.PredictFuturePosition(_minTimeToCollision);
            var movement = futurePosition - Vehicle.Position;

#if ANNOTATE_NAVMESH
            Debug.DrawRay(Vehicle.Position, movement, Color.cyan);
#endif

            if (_offMeshCheckingEnabled)
            {
                var probePosition = Vehicle.Position + _probePositionOffset;

                Profiler.BeginSample("Off-mesh checking");
                NavMesh.SamplePosition(probePosition, out hit, _probeRadius, _navMeshLayerMask);
                Profiler.EndSample();

                if (!hit.hit)
                {
                    // we're not on the navmesh
                    Profiler.BeginSample("Find closest edge");
                    NavMesh.FindClosestEdge(probePosition, out hit, _navMeshLayerMask);
                    Profiler.EndSample();

                    if (hit.hit)
                    {
                        // closest edge found
#if ANNOTATE_NAVMESH
                        Debug.DrawLine(probePosition, hit.position, Color.red);
#endif

                        return (hit.position - probePosition).normalized * Vehicle.MaxForce;
                    } // no closest edge - too far off the mesh
#if ANNOTATE_NAVMESH
                    Debug.DrawLine(probePosition, probePosition + Vector3.up * 3, Color.red);
#endif

                    return Vector3.zero;
                }
            }


            Profiler.BeginSample("NavMesh raycast");
            NavMesh.Raycast(Vehicle.Position, futurePosition, out hit, _navMeshLayerMask);
            Profiler.EndSample();

            if (!hit.hit)
                return Vector3.zero;

            Profiler.BeginSample("Calculate NavMesh avoidance");
            var moveDirection = Vehicle.Velocity.normalized;
            var avoidance = OpenSteerUtility.PerpendicularComponent(hit.normal, moveDirection);

            avoidance.Normalize();

#if ANNOTATE_NAVMESH
            Debug.DrawLine(Vehicle.Position, Vehicle.Position + avoidance, Color.white);
#endif

            avoidance += moveDirection * Vehicle.MaxForce * _avoidanceForceFactor;

#if ANNOTATE_NAVMESH
            Debug.DrawLine(Vehicle.Position, Vehicle.Position + avoidance, Color.yellow);
#endif

            Profiler.EndSample();

            return avoidance;
        }
    }
}