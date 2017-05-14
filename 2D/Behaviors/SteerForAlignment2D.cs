using UnityEngine;

namespace UnitySteer2D.Behaviors
{
    /// <summary>
    /// Steers a vehicle in alignment with detected neighbors
    /// </summary>
    /// <seealso cref="SteerForMatchingVelocity"/>
    [AddComponentMenu("UnitySteer2D/Steer/... for Alignment (Neighbour)")]
    [RequireComponent(typeof (SteerForNeighborGroup2D))]
    public class SteerForAlignment2D : SteerForNeighbors2D
    {
        public override Vector2 CalculateNeighborContribution(Vehicle2D other)
        {
            // accumulate sum of neighbor's heading
            return other.Forward;
        }
    }
}