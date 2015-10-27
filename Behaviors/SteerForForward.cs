// TODO does this need to be changed?

using UnityEngine;

namespace UnitySteer.Behaviors
{
    /// <summary>
    /// Trivial example that simply makes the vehicle move towards its forward vector
    /// </summary>
    [AddComponentMenu("UnitySteer/Steer/... for Forward")]
    public class SteerForForward : Steering
    {
        private Vector2 _desiredForward = Vector2.zero;

        private bool _overrideForward;

        /// <summary>
        /// Desired forward vector. If set to Vector3.zero we will steer toward the transform's forward
        /// </summary>
        public Vector2 DesiredForward
        {
            get { return _overrideForward ? _desiredForward : (Vector2)Vehicle.Transform.up; }
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
            return _overrideForward ? DesiredForward : (Vector2)Vehicle.Transform.up;
        }
    }
}