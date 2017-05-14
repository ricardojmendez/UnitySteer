//#define TRACE_ADJUSTMENTS

using System;
using UnityEngine;

namespace UnitySteer2D.Behaviors
{
    /// <summary>
    /// Vehicle subclass oriented towards vehicles that are controlled by
    /// an separate method, and meant to just provide an interface to obtain
    /// their speed and direction.
    /// </summary>
    /// <remarks>
    /// It assumes that the character will move in the direction of its 
    /// forward vector.  If it were to move like a biped, the implementation
    /// of the Velocity accessor should be changed.
    /// </remarks>
    [AddComponentMenu("UnitySteer2D/Objects/Passive Vehicle")]
    public class PassiveVehicle2D : Vehicle2D
    {
        #region Internal state values

        /// <summary>
        /// The magnitude of the last velocity vector assigned to the vehicle 
        /// </summary>
        private float _speed = 0;

        /// <summary>
        /// The vehicle's current velocity vector
        /// </summary>
        private Vector2 _velocity = Vector2.zero;

        /// <summary>
        /// A toggle to get the proper velocity vector.
        /// </summary>
        [SerializeField]
        private bool _isBiped;

        private Vector2 _lastPosition = Vector2.zero;

        #endregion

        /// <summary>
        /// Current vehicle speed
        /// </summary>  
        /// <remarks>
        /// If the vehicle has a speedometer, then we return the actual measured
        /// value instead of simply the length of the velocity vector.
        /// </remarks>
        public override float Speed
        {
            get { return Speedometer2D == null ? _speed : Speedometer2D.Speed; }
        }

        /// <summary>
        /// Current vehicle velocity
        /// </summary>
        public override Vector2 Velocity
        {
            get { return _isBiped ? _velocity : Forward * _speed; }
            protected set { throw new NotSupportedException("Cannot set the velocity directly on PassiveVehicle2D"); }
        }

        private void Update()
        {
            if (!CanMove)
            {
                Velocity = Vector2.zero; //Doesn't this throw an exception constantly?
            }
            else if (Position != _lastPosition)
            {
                _velocity = Position - _lastPosition;
                _lastPosition = Position;
            }
            else
            {
                _velocity = Vector2.zero;
            }
        }
    }
}