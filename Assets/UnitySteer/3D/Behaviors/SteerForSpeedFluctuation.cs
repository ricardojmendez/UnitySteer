using UnityEngine;

namespace UnitySteer.Behaviors
{
    /// <summary>
    /// Sample post-processing behavior that adds noise to the desired speed
    /// </summary>
    /// <remarks>
    /// This will have a significant effect on performance if you have a few
    /// thousand agents, so be warned.
    /// </remarks>
    [AddComponentMenu("UnitySteer/Steer/... for SpeedFluctuation (Post-process)")]
    public class SteerForSpeedFluctuation : Steering
    {
        [SerializeField] private float _noiseImpact = 0.5f;

        public override bool IsPostProcess
        {
            get { return true; }
        }

        /// <summary>
        /// Calculates the force to apply to the vehicle to reach a point
        /// </summary>
        /// <returns>
        /// A <see cref="Vector3"/>
        /// </returns>
        protected override Vector3 CalculateForce()
        {
            return Vehicle.DesiredVelocity * _noiseImpact *
                   (1.5f - Mathf.PerlinNoise(Time.time, Vehicle.MovementPriority));
        }
    }
}