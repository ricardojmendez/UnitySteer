#define SUPPORT_2D

using UnityEngine;

namespace UnitySteer.Behaviors
{
    /// <summary>
    /// Steers a vehicle to keep within a certain range of a point
    /// </summary>
    [AddComponentMenu("UnitySteer/Steer/... for Tether")]
    public class SteerForTether : Steering
    {
        #region Private properties

        [SerializeField] private float _maximumDistance = 30f;

#if SUPPORT_2D
        [SerializeField] private Vector2 _tetherPosition;
#else
        [SerializeField] private Vector3 _tetherPosition;
#endif

        #endregion

        #region Public properties

        public override bool IsPostProcess
        {
            get { return true; }
        }

        public float MaximumDistance
        {
            get { return _maximumDistance; }
            set { _maximumDistance = Mathf.Clamp(value, 0, float.MaxValue); }
        }

#if SUPPORT_2D
        public Vector2 TetherPosition
#else
        public Vector3 TetherPosition
#endif
        {
            get { return _tetherPosition; }
            set { _tetherPosition = value; }
        }

        #endregion

#if SUPPORT_2D
        protected override Vector2 CalculateForce()
        {
            var steering = Vector2.zero;
#else
        protected override Vector3 CalculateForce()
        {
            var steering = Vector3.zero;
#endif

            var difference = TetherPosition - Vehicle.Position;
            var distance = difference.magnitude;
            if (distance > _maximumDistance)
            {
                steering = (difference + Vehicle.DesiredVelocity) / 2;
            }
            return steering;
        }
    }
}