using UnityEngine;

namespace UnitySteer2D.Behaviors
{
    /// <summary>
    /// Behavior that will aim to achieve a constant speed along the vehicle's forward vector
    /// </summary>
    [AddComponentMenu("UnitySteer2D/Steer/... for Speed")]
    public class SteerForSpeed2D : Steering2D
    {
        /// <summary>
        /// Speed that the behavior will aim to achieve
        /// </summary>
        [SerializeField] private float _targetSpeed = 5;


        public float TargetSpeed
        {
            get { return _targetSpeed; }
            set { _targetSpeed = value; }
        }

        /// <summary>
        /// Calculates the force to apply to a vehicle to reach a target transform
        /// </summary>
        /// <returns>
        /// Force to apply <see cref="Vector2"/>
        /// </returns>
        protected override Vector2 CalculateForce()
        {
            return Vehicle.GetTargetSpeedVector(TargetSpeed);
        }
    }
}