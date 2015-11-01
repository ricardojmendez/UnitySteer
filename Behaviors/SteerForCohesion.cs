// #define DEBUG_COMFORT_DISTANCE
#define SUPPORT_2D

using UnityEngine;

namespace UnitySteer.Behaviors
{
    /// <summary>
    /// Steers a vehicle to remain in cohesion with neighbors
    /// </summary>
    [AddComponentMenu("UnitySteer/Steer/... for Cohesion")]
    [RequireComponent(typeof (SteerForNeighborGroup))]
    public class SteerForCohesion : SteerForNeighbors
    {
#if SUPPORT_2D
        public override Vector2 CalculateNeighborContribution(Vehicle other)
        {
            // accumulate sum of forces leading us towards neighbor's positions
            var distance = other.Position - Vehicle.Position;
            var sqrMag = distance.sqrMagnitude;
            // Provide some contribution, but diminished by the distance to 
            // the vehicle.
            distance *= 1 / sqrMag;
            return distance;
        }
#else
        public override Vector3 CalculateNeighborContribution(Vehicle other)
        {
            // accumulate sum of forces leading us towards neighbor's positions
            var distance = other.Position - Vehicle.Position;
            var sqrMag = distance.sqrMagnitude;
            // Provide some contribution, but diminished by the distance to 
            // the vehicle.
            distance *= 1 / sqrMag;
            return distance;
        }
#endif

        private void OnDrawGizmos()
        {
#if DEBUG_COMFORT_DISTANCE
		Gizmos.color = Color.magenta;
		Gizmos.DrawWireSphere(transform.position, ComfortDistance);
#endif
        }
    }
}