// #define DEBUG_COMFORT_DISTANCE

using UnityEngine;

namespace UnitySteer2D.Behaviors
{
    /// <summary>
    /// Steers a vehicle to remain in cohesion with neighbors
    /// </summary>
    [AddComponentMenu("UnitySteer2D/Steer/... for Cohesion (Neighbour)")]
    [RequireComponent(typeof (SteerForNeighborGroup2D))]
    public class SteerForCohesion2D : SteerForNeighbors2D
    {
        public override Vector2 CalculateNeighborContribution(Vehicle2D other)
        {
            // accumulate sum of forces leading us towards neighbor's positions
            var distance = other.Position - Vehicle.Position;
            var sqrMag = distance.sqrMagnitude;
            // Provide some contribution, but diminished by the distance to 
            // the vehicle.
            distance *= 1 / sqrMag;
            return distance;
        }

        private void OnDrawGizmos()
        {
#if DEBUG_COMFORT_DISTANCE
		Gizmos.color = Color.magenta;
		Gizmos.DrawWireSphere(transform.position, ComfortDistance);
#endif
        }
    }
}