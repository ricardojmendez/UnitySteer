using System;
using UnityEngine;

namespace UnitySteer.Behaviors
{
    /// <summary>
    /// Vehicle subclass which automatically applies the steering forces from
    /// the components attached to the object.  AutonomousVehicle is characterized
    /// for the vehicle always moving in the same direction as its forward vector,
    /// unlike Bipeds which are able to side-step.
    /// </summary>
    [AddComponentMenu("UnitySteer/Vehicle/Autonomous")]
    public class AutonomousVehicle : TickedVehicle
    {
        #region Internal state values

        private float _speed;

        #endregion

        /// <summary>
        /// Acceleration rate - it'll be used as a multiplier for the speed
        /// at which the velocity is interpolated when accelerating. A rate
        /// of 1 means that we interpolate across 1 second; a rate of 5 means
        /// we do it five times as fast.
        /// </summary>
        [SerializeField] private float _accelerationRate = 5;

        /// <summary>
        /// Deceleration rate - it'll be used as a multiplier for the speed
        /// at which the velocity is interpolated when decelerating. A rate
        /// of 1 means that we interpolate across 1 second; a rate of 5 means
        /// we do it five times as fast.
        /// </summary>
        [SerializeField] private float _decelerationRate = 8;


        /// <summary>
        /// Current vehicle speed
        /// </summary>
        public override float Speed
        {
            get { return _speed; }
        }

        /// <summary>
        /// Current vehicle velocity
        /// </summary>
        public override Vector3 Velocity
        {
            get { return Transform.forward * Speed; }
            protected set { throw new NotSupportedException("Cannot set the velocity directly on AutonomousVehicle"); }
        }

        #region Speed-related methods

        /// <summary>
        /// Uses a desired velocity vector to adjust the vehicle's target speed and 
        /// orientation velocity.
        /// </summary>
        /// <param name="velocity">Newly calculated velocity</param>
        protected override void SetCalculatedVelocity(Vector3 velocity)
        {
            TargetSpeed = velocity.magnitude;
            OrientationVelocity = Mathf.Approximately(_speed, 0) ? Transform.forward : velocity / TargetSpeed;
        }

        /// <summary>
        /// Calculates how much the agent's position should change in a manner that
        /// is specific to the vehicle's implementation.
        /// </summary>
        /// <param name="deltaTime">Time delta to use in position calculations</param>
        protected override Vector3 CalculatePositionDelta(float deltaTime)
        {
            /*
		 * Notice that we clamp the target speed and not the speed itself, 
		 * because the vehicle's maximum speed might just have been lowered
		 * and we don't want its actual speed to suddenly drop.
		 */
            var targetSpeed = Mathf.Clamp(TargetSpeed, 0, MaxSpeed);
            if (Mathf.Approximately(_speed, targetSpeed))
            {
                _speed = targetSpeed;
            }
            else
            {
                var rate = TargetSpeed > _speed ? _accelerationRate : _decelerationRate;
                _speed = Mathf.Lerp(_speed, targetSpeed, deltaTime * rate);
            }

            return Velocity * deltaTime;
        }

        /// <summary>
        /// Zeros this vehicle's target speed, which results on its desired velocity
        /// being zero.
        /// </summary>
        protected override void ZeroVelocity()
        {
            TargetSpeed = 0;
        }

        #endregion
    }
}