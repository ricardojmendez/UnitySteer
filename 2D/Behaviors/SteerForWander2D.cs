using UnityEngine;

namespace UnitySteer2D.Behaviors
{
    /// <summary>
    /// Steers a vehicle to wander around within a maximum latitude for the side/up
    /// vectors. Speed changes are smoothed based on the Vehicle.DeltaTime, not
    /// Time.deltaTime, since steering behaviors may not be refreshed every frame.
    /// </summary>
    [AddComponentMenu("UnitySteer2D/Steer/... for Wander")]
    public class SteerForWander2D : Steering2D
    {
        #region Private fields

        private float _wanderSide;
        private float _wanderUp;

        [SerializeField] private float _maxLatitudeSide = 2;
        [SerializeField] private float _maxLatitudeUp = 2;

        /// <summary>
        /// The smooth rate per second to apply to the random walk value during blending.
        /// </summary>
        [SerializeField] private float _smoothRate = 0.05f;

        #endregion

        #region Public properties

        /// <summary>
        /// Maximum latitude to use for the random scalar walk on the side
        /// </summary>
        public float MaxLatitudeSide
        {
            get { return _maxLatitudeSide; }
            set { _maxLatitudeSide = value; }
        }

        /// <summary>
        /// Maximum latitude to use for the random scalar walk on the up vector
        /// </summary>
        public float MaxLatitudeUp
        {
            get { return _maxLatitudeUp; }
            set { _maxLatitudeUp = value; }
        }

        #endregion

        protected override Vector2 CalculateForce()
        {
            var speed = Vehicle.MaxSpeed;

            // random walk WanderSide and WanderUp between the specified latitude
            var randomSide = OpenSteerUtility.ScalarRandomWalk(_wanderSide, speed, -_maxLatitudeSide, _maxLatitudeSide);
            var randomUp = OpenSteerUtility.ScalarRandomWalk(_wanderUp, speed, -_maxLatitudeUp, _maxLatitudeUp);
            _wanderSide = Mathf.Lerp(_wanderSide, randomSide, _smoothRate * Vehicle.DeltaTime);
            _wanderUp = Mathf.Lerp(_wanderUp, randomUp, _smoothRate * Vehicle.DeltaTime);

            var sideVector = (Vector2)(Quaternion.Euler(0, 0, 90) * Vehicle.Forward) * _wanderSide; //this vector is to the right of whatever direction is chosen for Forward.

            var upVector = Vehicle.Forward * _wanderUp;

            var result = sideVector + upVector;

            return result;
        }
    }
}
