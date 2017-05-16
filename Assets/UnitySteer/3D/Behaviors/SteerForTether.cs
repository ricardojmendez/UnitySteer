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
        [SerializeField] private Vector3 _tetherPosition;

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

        public Vector3 TetherPosition
        {
            get { return _tetherPosition; }
            set { _tetherPosition = value; }
        }

        #endregion

        protected override Vector3 CalculateForce()
        {
            var steering = Vector3.zero;

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