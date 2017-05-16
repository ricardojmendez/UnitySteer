using UnityEngine;

namespace UnitySteer2D.Behaviors
{
    /// <summary>
    /// Steers a vehicle to avoid another Rigidbody2D (very basic future position prediction)
    /// </summary>
    [AddComponentMenu("UnitySteer2D/Steer/... for Rigidbody Evasion")]
    public class SteerForRigidbodyEvasion2D : Steering2D
    {
        #region Private fields

        [SerializeField]
        private Rigidbody2D _menace;

        [SerializeField]
        private float _predictionTime;

        #endregion

        #region Public properties

        /// <summary>
        /// How many seconds to look ahead for position prediction
        /// </summary>
        public float PredictionTime
        {
            get { return _predictionTime; }
            set { _predictionTime = value; }
        }

        /// <summary>
        /// Vehicle menace
        /// </summary>
        public Rigidbody2D Menace
        {
            get { return _menace; }
            set { _menace = value; }
        }

        #endregion

        protected override Vector2 CalculateForce()
        {
            // offset from this to menace, that distance, unit vector toward menace
            var offset = _menace.position - Vehicle.Position;
            var distance = offset.magnitude;

            var roughTime = distance / _menace.velocity.magnitude;
            var predictionTimeUsed = Mathf.Min(_predictionTime, roughTime);
            var target = _menace.position + (_menace.velocity * predictionTimeUsed);

            // This was the totality of SteerToFlee
            return Vehicle.Position - target;
        }
    }
}