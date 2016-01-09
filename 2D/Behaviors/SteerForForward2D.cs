using UnityEngine;

namespace UnitySteer2D.Behaviors
{
    /// <summary>
    /// Trivial example that simply makes the vehicle move towards its forward vector
    /// </summary>
    [AddComponentMenu("UnitySteer2D/Steer/... for Forward")]
    public class SteerForForward2D : Steering2D
    {
        private Vector2 _desiredForward = Vector2.zero;

        private bool _overrideForward;

        /// <summary>
        /// Desired forward vector. If set to Vector2.zero we will steer toward the vehicle's forward.
        /// </summary>
        public Vector2 DesiredForward
        {
            get { return _overrideForward ? _desiredForward : Vehicle.Forward; }
            set
            {
                _desiredForward = value;
                _overrideForward = value != Vector2.zero;
            }
        }

        /// <summary>
        /// Calculates the force to apply to the vehicle to reach a point
        /// </summary>
        /// <returns>
        /// A <see cref="Vector2"/>
        /// </returns>
        protected override Vector2 CalculateForce()
        {
            return _overrideForward ? DesiredForward : Vehicle.Forward;
        }
    }
}