#define SUPPORT_2D

using UnityEngine;

namespace UnitySteer.Behaviors
{
    [AddComponentMenu("UnitySteer/Steer/... for Point")]
    public class SteerForPoint : Steering
    {
        /// <summary>
        /// Target point
        /// </summary>
        /// <remarks>
        /// Declared as a separate value so that we can inspect it on Unity in 
        /// debug mode.
        /// </remarks>
#if SUPPORT_2D
        [SerializeField] private Vector2 _targetPoint = Vector2.zero;
#else
        [SerializeField] private Vector3 _targetPoint = Vector3.zero;
#endif

        /// <summary>
        /// Should the vehicle's velocity be considered in the seek calculations?
        /// </summary>
        /// <remarks>
        /// If true, the vehicle will slow down as it approaches its target. See
        /// the remarks on GetSeekVector.
        /// </remarks>
        [SerializeField] private bool _considerVelocity;

        /// <summary>
        /// Should the target default to the vehicle current position if it's set to Vector3.zero?
        /// </summary>
        [SerializeField] private bool _defaultToCurrentPosition = true;


        /// <summary>
        /// The target point.
        /// </summary>
#if SUPPORT_2D
        public Vector2 TargetPoint
#else
        public Vector3 TargetPoint
#endif
        {
            get { return _targetPoint; }
            set
            {
                if (_targetPoint == value) return;
                _targetPoint = value;
                ReportedArrival = false;
            }
        }

        /// <summary>
        /// Should the vehicle's velocity be considered in the seek calculations?
        /// </summary>
        /// <remarks>
        /// If true, the vehicle will slow down as it approaches its target
        /// </remarks>
        public bool ConsiderVelocity
        {
            get { return _considerVelocity; }
            set { _considerVelocity = value; }
        }

        protected override void Start()
        {
            base.Start();

#if SUPPORT_2D
            if (_defaultToCurrentPosition && TargetPoint == Vector2.zero)
#else
            if (_defaultToCurrentPosition && TargetPoint == Vector3.zero)
#endif
            {
                enabled = false;
            }
        }

        /// <summary>
        /// Calculates the force to apply to the vehicle to reach a point
        /// </summary>
        /// <returns>
        /// A <see cref="Vector2"/>
        /// </returns>
#if SUPPORT_2D
        protected override Vector2 CalculateForce()
#else
        protected override Vector3 CalculateForce()
#endif
        {
            return Vehicle.GetSeekVector(TargetPoint, _considerVelocity);
        }
    }
}