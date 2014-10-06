using UnityEngine;

namespace UnitySteer.Behaviors
{
    /// <summary>
    /// Behavior that will aim to achieve a constant speed along the vehicle's forward vector
    /// </summary>
    [AddComponentMenu("UnitySteer/Steer/... for Speed")]
    public class SteerForSpeed : Steering
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
        /// Force to apply <see cref="Vector3"/>
        /// </returns>
        protected override Vector3 CalculateForce()
        {
            return Vehicle.GetTargetSpeedVector(TargetSpeed);
        }
    }
}