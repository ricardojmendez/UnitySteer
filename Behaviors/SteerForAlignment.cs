#define SUPPORT_2D

using UnityEngine;

namespace UnitySteer.Behaviors
{
    /// <summary>
    /// Steers a vehicle in alignment with detected neighbors
    /// </summary>
    /// <seealso cref="SteerForMatchingVelocity"/>
    [AddComponentMenu("UnitySteer/Steer/... for Alignment")]
    [RequireComponent(typeof (SteerForNeighborGroup))]
    public class SteerForAlignment : SteerForNeighbors
    {
#if SUPPORT_2D
        public override Vector2 CalculateNeighborContribution(Vehicle other)
        {
            // accumulate sum of neighbor's heading
            return other.Forward;
        }
#else
        public override Vector3 CalculateNeighborContribution(Vehicle other)
        {
            // accumulate sum of neighbor's heading
            return other.Forward;
        }
#endif
    }
}