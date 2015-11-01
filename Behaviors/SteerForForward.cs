// TODO does this need to be changed?
#define SUPPORT_2D


using UnityEngine;

namespace UnitySteer.Behaviors
{
    /// <summary>
    /// Trivial example that simply makes the vehicle move towards its forward vector
    /// </summary>
    [AddComponentMenu("UnitySteer/Steer/... for Forward")]
    public class SteerForForward : Steering
    {
#if SUPPORT_2D
        private Vector2 _desiredForward = Vector2.zero;
#else
        private Vector3 _desiredForward = Vector3.zero;
#endif

        private bool _overrideForward;

        /// <summary>
        /// Desired forward vector. If set to Vector3.zero we will steer toward the transform's forward
        /// </summary>
#if SUPPORT_2D
        public Vector2 DesiredForward
        {
            get { return _overrideForward ? _desiredForward : Vehicle.Forward; }
            set
            {
                _desiredForward = value;
                _overrideForward = value != Vector2.zero;
            }
        }
#else
        public Vector3 DesiredForward
        {
            get { return _overrideForward ? _desiredForward : Vehicle.Forward; }
            set
            {
                _desiredForward = value;
                _overrideForward = value != Vector3.zero;
            }
        }
#endif

        /// <summary>
        /// Calculates the force to apply to the vehicle to reach a point
        /// </summary>
        /// <returns>
        /// A <see cref="Vector2"/>
        /// </returns>
#if SUPPORT_2D
        protected override Vector2 CalculateForce()
        {
            return _overrideForward ? DesiredForward : Vehicle.Forward;
        }
#else
        protected override Vector3 CalculateForce()
        {
            return _overrideForward ? DesiredForward : Vehicle.Forward;
        }
#endif
    }
}