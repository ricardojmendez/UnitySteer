using UnityEngine;

namespace UnitySteer2D.Behaviors
{
    /// <summary>
    /// Steers a vehicle in alignment with detected neighbors
    /// </summary>
    /// <seealso cref="SteerForMatchingVelocity"/>
    [AddComponentMenu("UnitySteer2D/Steer/... for Alignment (Neighbour)")]
    [RequireComponent(typeof (SteerForNeighborGroup))]
    public class SteerForAlignment : SteerForNeighbors
    {
        public override Vector2 CalculateNeighborContribution(Vehicle other)
        {
            // accumulate sum of neighbor's heading
            return other.Forward;
        }
    }
}