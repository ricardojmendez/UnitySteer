#define ANNOTATE_PURSUIT
using UnityEngine;

namespace UnitySteer.Behaviors
{
    /// <summary>
    /// Steers a vehicle to pursue another one
    /// </summary>
    [AddComponentMenu("UnitySteer/Steer/... for Pursuit")]
    public class SteerForPursuit : Steering
    {
        #region Private fields

        /// <summary>
        /// A distance at which we are 'close enough' to stop pursuing
        /// </summary>
        /// <remarks>
        /// Notice that this is different from the vehicle's arrival radius,
        /// since we may want to be able to have an attack distance that is
        /// different from the radius used when moving to a point.
        /// </remarks>
        [SerializeField] private float _acceptableDistance;

        /// <summary>
        /// Should the vehicle consider its own speed when approaching the quarry?
        /// </summary>
        [SerializeField] private bool _slowDownOnApproach;

        [SerializeField] private DetectableObject _quarry;

        [SerializeField] private float _maxPredictionTime = 5;

        #endregion

        #region Public properties

        public float AcceptableDistance
        {
            get { return _acceptableDistance; }
            set { _acceptableDistance = value; }
        }

        /// <summary>
        /// Maximum time to look ahead for the prediction calculation
        /// </summary>
        public float MaxPredictionTime
        {
            get { return _maxPredictionTime; }
            set { _maxPredictionTime = value; }
        }

        /// <summary>
        /// Target being pursued
        /// </summary>
        /// <remarks>When set, it will clear the flag that indicates we've already reported that we arrived</remarks>
        public DetectableObject Quarry
        {
            get { return _quarry; }
            set
            {
                if (_quarry != value)
                {
                    ReportedArrival = false;
                    _quarry = value;
                }
            }
        }

        #endregion

        protected override Vector3 CalculateForce()
        {
            if (_quarry == null)
            {
                enabled = false;
                return Vector3.zero;
            }

            var force = Vector3.zero;
            var offset = _quarry.Position - Vehicle.Position;
            var distance = offset.magnitude;
            var radius = Vehicle.Radius + _quarry.Radius + _acceptableDistance;

            if (!(distance > radius)) return force;

            var unitOffset = offset / distance;

            // how parallel are the paths of "this" and the quarry
            // (1 means parallel, 0 is pependicular, -1 is anti-parallel)
            var parallelness = Vector3.Dot(transform.forward, _quarry.transform.forward);

            // how "forward" is the direction to the quarry
            // (1 means dead ahead, 0 is directly to the side, -1 is straight back)
            var forwardness = Vector3.Dot(transform.forward, unitOffset);

            var directTravelTime = distance / Vehicle.Speed;
            // While we could parametrize this value, if we care about forward/backwards
            // these values are appropriate enough.
            var f = OpenSteerUtility.IntervalComparison(forwardness, -0.707f, 0.707f);
            var p = OpenSteerUtility.IntervalComparison(parallelness, -0.707f, 0.707f);

            float timeFactor = 0; // to be filled in below

            // Break the pursuit into nine cases, the cross product of the
            // quarry being [ahead, aside, or behind] us and heading
            // [parallel, perpendicular, or anti-parallel] to us.
            switch (f)
            {
                case +1:
                    switch (p)
                    {
                        case +1: // ahead, parallel
                            timeFactor = 4;
                            break;
                        case 0: // ahead, perpendicular
                            timeFactor = 1.8f;
                            break;
                        case -1: // ahead, anti-parallel
                            timeFactor = 0.85f;
                            break;
                    }
                    break;
                case 0:
                    switch (p)
                    {
                        case +1: // aside, parallel
                            timeFactor = 1;
                            break;
                        case 0: // aside, perpendicular
                            timeFactor = 0.8f;
                            break;
                        case -1: // aside, anti-parallel
                            timeFactor = 4;
                            break;
                    }
                    break;
                case -1:
                    switch (p)
                    {
                        case +1: // behind, parallel
                            timeFactor = 0.5f;
                            break;
                        case 0: // behind, perpendicular
                            timeFactor = 2;
                            break;
                        case -1: // behind, anti-parallel
                            timeFactor = 2;
                            break;
                    }
                    break;
            }

            // estimated time until intercept of quarry
            var et = directTravelTime * timeFactor;
            var etl = (et > _maxPredictionTime) ? _maxPredictionTime : et;

            // estimated position of quarry at intercept
            var target = _quarry.PredictFuturePosition(etl);

            force = Vehicle.GetSeekVector(target, _slowDownOnApproach);

#if ANNOTATE_PURSUIT
            Debug.DrawRay(Vehicle.Position, force, Color.blue);
            Debug.DrawLine(Quarry.Position, target, Color.cyan);
            Debug.DrawRay(target, Vector3.up * 4, Color.cyan);
#endif

            return force;
        }
    }
}