using UnityEngine;

namespace UnitySteer.Behaviors
{
    /// <summary>
    /// Steers a vehicle to follow a path
    /// </summary>
    /// <remarks>
    /// Based on SteerToFollowPath.  It won't necessarily stick to the straight 
    /// line defined by the path segments, but will instead estimate a future 
    /// path position based on the _predictionTime property and aim for that 
    /// position. This means the vehicle is likely to cut corners if the 
    /// prediction time is too far ahead (which could be useful for spaceships).
    /// </remarks>
    [AddComponentMenu("UnitySteer/Steer/... for PathSimplified")]
    public class SteerForPathSimplified : Steering
    {
        #region Private fields

        /// <summary>
        /// How many seconds ahead to predict the vehicle's position in the 
        /// path in order to calculate the point to aim for.
        /// </summary>
        [SerializeField] private float _predictionTime = 1.5f;

        /// <summary>
        /// Minimum vehicle speed to consider when estimating future position.
        /// </summary>
        [SerializeField] private float _minSpeedToConsider = 0.25f;

        private IPathway _path;

        #endregion

        #region Public properties

        /// <summary>
        /// How far ahead to estimate our position
        /// </summary>
        public float PredictionTime
        {
            get { return _predictionTime; }
            set { _predictionTime = Mathf.Max(value, 0); }
        }

        /// <summary>
        /// How far along the path we were the last time we calculated forces?
        /// </summary>
        /// <value>The last path distance evaluated.</value>
        public float DistanceAlongPath { get; private set; }


        /// <summary>
        /// What percentage of the path we had traversed when we last evaluated?
        /// </summary>
        /// <value>The last path percentage evaluated.</value>
        public float PathPercentTraversed
        {
            get { return (Path != null && Path.TotalPathLength > 0) ? DistanceAlongPath / Path.TotalPathLength : 0; }
        }

        /// <summary>
        /// Minimum speed to consider when predicting the future position. If the
        /// vehicle's speed is under this value, estimates will instead be done
        /// at this value plus the prediction time.
        /// </summary>
        public float MinSpeedToConsider
        {
            get { return _minSpeedToConsider; }
            set { _minSpeedToConsider = value; }
        }

        /// <summary>
        /// Path to follow
        /// </summary>
        public IPathway Path
        {
            get { return _path; }
            set
            {
                _path = value;
                DistanceAlongPath = 0;
            }
        }

        #endregion

        /// <summary>
        /// Should the force be calculated?
        /// </summary>
        /// <returns>
        /// A <see cref="Vector3"/>
        /// </returns>
        protected override Vector3 CalculateForce()
        {
            if (Path == null || Path.SegmentCount < 2)
            {
                return Vector3.zero;
            }

            // If the vehicle's speed is 0, use a low speed for future position
            // calculation. Otherwise the vehicle will remain where it is if he
            // starts within the path, because its current position matches its
            // future path position
            var speed = Mathf.Max(Vehicle.Speed, _minSpeedToConsider);

            // our goal will be offset from our path distance by this amount
            var pathDistanceOffset = _predictionTime * speed;

            // measure distance along path of our current and predicted positions
            DistanceAlongPath = Path.MapPointToPathDistance(Vehicle.Position);

            /*
		 * Otherwise we need to steer towards a target point obtained
		 * by adding pathDistanceOffset to our current path position.
		 * 
		 * Notice that this method does not steer for the point in the
		 * path that is closest to our future position, which is why 
		 * we don't calculate the closest point in the path to our future
		 * position. 
		 * 
		 * Instead, it estimates how far the vehicle will move in units, 
		 * and then aim for the point in the path that is that many units 
		 * away from our current path position _in path length_.   This 
		 * means that it adds up the segment lengths and aims for the point 
		 * that is N units along the length of the path, which can imply
		 * bends and turns and is not a straight vector projected away
		 * from our position.
		 */
            var targetPathDistance = DistanceAlongPath + pathDistanceOffset;
            var target = Path.MapPathDistanceToPoint(targetPathDistance);

            /*
		 * Return steering to seek target on path.
		 * 
		 * If you set the considerVelocity parameter to true, it'll slow
		 * down at each target to try to ease its arrival, which will 
		 * likely cause it to come to a stand still at low prediction
		 * times.
		 */
            var seek = Vehicle.GetSeekVector(target);

            if (seek == Vector3.zero && targetPathDistance <= Path.TotalPathLength)
            {
                /*
			 * If we should not displace but still have some distance to go,
			 * that means that we've encountered an edge case: a relatively low
			 * vehicle speed and short prediction range, combined with a path
			 * that twists. In that case, it's possible that the predicted future
			 * point just around the bend is still within the vehicle's arrival
			 * radius.  In that case, aim a bit further beyond the vehicle's 
			 * arrival radius so that it can continue moving.
			 */
                target = Path.MapPathDistanceToPoint(targetPathDistance + 2f * Vehicle.ArrivalRadius);
                seek = Vehicle.GetSeekVector(target);
            }

            return seek;
        }

        protected void OnDrawGizmosSelected()
        {
            if (Path != null)
            {
                Path.DrawGizmos();
            }
        }
    }
}