using UnityEngine;

namespace UnitySteer2D.Behaviors
{
    /// <summary>
    /// Post-processing behavior that ensures that the vehicle always moves 
    /// at at least a minimum speed
    /// </summary>
    /// <remarks>
    /// This could also be done as a property of TickedVehicle, but adding it
    /// as a post-processing behavior for now to have it be more modular.
    /// </remarks>
    [AddComponentMenu("UnitySteer2D/Steer/... for Minimum Speed (Post-Process)")]
    public class SteerForMinimumSpeed2D : Steering2D
    {
        [SerializeField] private float _minimumSpeed = 4;

        [SerializeField] private bool _moveForwardWhenZero = true;

        public override bool IsPostProcess
        {
            get { return true; }
        }

        public float MinimumSpeed
        {
            get { return _minimumSpeed; }
            set { _minimumSpeed = value; }
        }


        /// <summary>
        /// Calculates the force to apply to the vehicle to reach a point
        /// </summary>
        /// <returns>
        /// A <see cref="Vector2"/>
        /// </returns>
        protected override Vector2 CalculateForce()
        {
            var result = Vehicle.DesiredVelocity;
            if (_moveForwardWhenZero && Mathf.Approximately(Vehicle.TargetSpeed, 0))
            {
                result = Vehicle.Forward * _minimumSpeed;
            }
            else if (Vehicle.TargetSpeed < _minimumSpeed)
            {
                result = Vehicle.DesiredVelocity.normalized * _minimumSpeed;
            }
            return result;
        }
    }
}