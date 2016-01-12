using UnityEngine;

namespace UnitySteer2D.Behaviors
{
    /// <summary>
    /// Steers a vehicle to keep within a certain range of a point
    /// </summary>
    [AddComponentMenu("UnitySteer2D/Steer/... for Tether (Post-Process)")]
    public class SteerForTether2D : Steering2D
    {
        #region Private properties

        [SerializeField] private float _maximumDistance = 30f;

        [SerializeField] private Vector2 _tetherPosition;

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

        public Vector2 TetherPosition
        {
            get { return _tetherPosition; }
            set { _tetherPosition = value; }
        }

        #endregion

        protected override Vector2 CalculateForce()
        {
            var steering = Vector2.zero;
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