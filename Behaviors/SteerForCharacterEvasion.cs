// TODO CharacterController is useless for 2D, could be turned into a rigidbody or a detectableobject (mines?) since we already have a vehicle evasion.
#define SUPPORT_2D
#define OBJECTTOGGLE

using UnityEngine;

namespace UnitySteer.Behaviors
{
#if !SUPPORT_2D
    /// <summary>
    /// Steers a vehicle to avoid another CharacterController (very basic future position prediction)
    /// </summary>
    [AddComponentMenu("UnitySteer/Steer/... for Character Evasion")]
    public class SteerForCharacterEvasion : Steering
    {
    #region Private fields

        [SerializeField] private CharacterController _menace;

        [SerializeField] private float _predictionTime;

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
        public CharacterController Menace
        {
            get { return _menace; }
            set { _menace = value; }
        }

    #endregion

        protected override Vector3 CalculateForce()
        {
            // offset from this to menace, that distance, unit vector toward menace
            var offset = _menace.transform.position - Vehicle.Position;
            var distance = offset.magnitude;

            var roughTime = distance / _menace.velocity.magnitude;
			var predictionTimeUsed = Mathf.Min(_predictionTime, roughTime);
            var target = _menace.transform.position + (_menace.velocity * predictionTimeUsed);

            // This was the totality of SteerToFlee
			return Vehicle.Position - target;
        }
    }
#elif !OBJECTTOGGLE
    /// <summary>
    /// Steers a vehicle to avoid another Rigidbody2D (very basic future position prediction)
    /// </summary>
    [AddComponentMenu("UnitySteer/Steer/... for Character Evasion")]
    public class SteerForCharacterEvasion : Steering
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
#else 
    /// <summary>
    /// Steers a vehicle to avoid another DetectableObject (stationary object, like a mine)
    /// </summary>
    /// <remarks>
    /// If this is indeed useful maybe put it into another file so it can be used alongside a RigidBody2D evasion.
    /// </remarks>
    [AddComponentMenu("UnitySteer/Steer/... for Character Evasion")]
    public class SteerForCharacterEvasion : Steering
    {
        #region Private fields

        [SerializeField]
        private DetectableObject _menace;

        #endregion

        #region Public properties

        /// <summary>
        /// Vehicle menace
        /// </summary>
        public DetectableObject Menace
        {
            get { return _menace; }
            set { _menace = value; }
        }

        #endregion

        protected override Vector2 CalculateForce()
        {
            var target = _menace.Position;

            // This was the totality of SteerToFlee
            return Vehicle.Position - target;
        }
    }
#endif
}