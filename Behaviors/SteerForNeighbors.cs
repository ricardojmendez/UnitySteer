using System;
using UnityEngine;

namespace UnitySteer.Behaviors
{
    /// <summary>
    /// Base class for behaviors which steer a vehicle in relation to detected neighbors.
    /// It does not return a force directly on CalculateForce, but instead just
    /// returns a neighbor contribution when queried by SteerForNeighborGroup.
    /// </summary>
    public abstract class SteerForNeighbors : Steering
    {
        #region Private serialized fields

        /// <summary>
        /// Minimum distance from the vehicle to a neighbor for the behavior to apply, otherwise it is skipped
        /// </summary>
        [SerializeField] private float _minDistance = 1;

        /// <summary>
        /// Maximum distance from the vehicle to a neighbor the behavior to apply, otherwise it is skipped
        /// </summary>
        [SerializeField] private float _maxDistance = 10;

        #endregion

        #region Public properties

        /// <summary>
        /// Minimum distance from the vehicle to a neighbor for the behavior to apply, otherwise it is skipped
        /// </summary>
        public float MinDistance
        {
            get { return _minDistance; }
            set
            {
                _minDistance = Mathf.Max(value, 0);
                MinDistanceSquared = _minDistance * _minDistance;
            }
        }

        /// <summary>
        /// Square of the MinDistance
        /// </summary>
        /// <see cref="MinDistance"/>
        public float MinDistanceSquared { get; private set; }

        /// <summary>
        /// Maximum distance from the vehicle to a neighbor the behavior to apply, otherwise it is skipped
        /// </summary>
        public float MaxDistance
        {
            get { return _maxDistance; }
            set
            {
                _maxDistance = Mathf.Max(value, 0);
                MaxDistanceSquared = _maxDistance * _maxDistance;
            }
        }

        /// <summary>
        /// Square of the MaxDistance
        /// </summary>
        /// <see cref="MaxDistance"/>
        public float MaxDistanceSquared { get; private set; }

        #endregion

        #region Methods

        protected override sealed Vector3 CalculateForce()
        {
            /*
         * You should never override this method nor change its behavior unless
         * youre 100% sure what you're doing.   See SteerForNeighborGroup.
         * 
         * Raise an exception if called. Everything will be calculated by
         * by SteerForNeighborGroup.
         */
            throw new NotImplementedException("SteerForNeighbors.CalculateForce should never be called directly.  " +
                                              "Did you enable a SteerForNeighbors subclass manually? They are disabled by SteerForNeighborGroup on Start.");
        }

        public abstract Vector3 CalculateNeighborContribution(Vehicle other);

        protected override void Awake()
        {
            base.Awake();
            MaxDistanceSquared = _maxDistance * _maxDistance;
            MinDistance = _minDistance * _minDistance;
        }

        /// <summary>
        /// Initialize this instance.
        /// </summary>
        /// <remarks>Used since SteerForNeighborGroup disables the behaviors, so
        /// Unity may end up not calling their Awake and Start methods.</remarks>
        public void Initialize()
        {
            Awake();
            Start();
        }

        /// <summary>
        /// Evaluates if a vehicle located in a certain direction is in range of this behavior.
        /// </summary>
        /// <param name="difference">A displacement from our position</param>
        /// <returns>True if in range, false otherwise</returns>
        public bool IsDirectionInRange(Vector3 difference)
        {
            return
                OpenSteerUtility.IntervalComparison(difference.sqrMagnitude, MinDistanceSquared, MaxDistanceSquared) ==
                0;
        }

        #endregion
    }
}