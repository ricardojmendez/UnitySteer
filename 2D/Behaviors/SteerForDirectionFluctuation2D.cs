using UnityEngine;

namespace UnitySteer2D.Behaviors
{
    /// <summary>
    /// Sample post-processing behavior that adds noise to the desired direction
    /// </summary>
    /// <remarks>
    /// This will have a significant effect on performance if you have a few
    /// thousand agents, so be warned.
    /// </remarks>
    [AddComponentMenu("UnitySteer2D/Steer/... for Direction Fluctuation (Post-Process)")]
    public class SteerForDirectionFluctuation2D : Steering2D
    {
        [SerializeField] private float _amount = 5f;

        // public override bool IsPostProcess
        // {
        //     get { return true; }
        // }

        /// <summary>
        /// Calculates the force to apply to the vehicle to reach a point
        /// </summary>
        /// <returns>
        /// A <see cref="Vector2"/>
        /// </returns>
        protected override Vector2 CalculateForce()
        {
            float noise = 2f*Mathf.PerlinNoise(Time.time, Vehicle.MovementPriority) - 1f;
            Vector2 left = Quaternion.AngleAxis(90f, Vector3.forward) * Vehicle.DesiredVelocity;
            left.Normalize();
            return left * noise * _amount;
        }
    }
}