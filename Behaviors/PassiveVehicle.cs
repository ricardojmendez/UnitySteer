#define TRACE_ADJUSTMENTS
using UnityEngine;
using UnitySteer;
using System.Linq;
using TickedPriorityQueue;

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
[AddComponentMenu("UnitySteer/Vehicle/Passive")]
public class PassiveVehicle : Vehicle {
    #region Internal state values
    /// <summary>
    /// The magnitude of the last velocity vector assigned to the vehicle 
    /// </summary>
    float _speed = 0;

    /// <summary>
    /// The biped's current velocity vector
    /// </summary>
    Vector3 _velocity;
    #endregion
 
    public  override bool CanMove {
        set { 
            base.CanMove = value;
            if (!CanMove) {
                Velocity = Vector3.zero;
            }
        }
    }
 
    /// <summary>
    /// Current vehicle speed
    /// </summary>  
    /// <remarks>
    /// If the vehicle has a speedometer, then we return the actual measured
    /// value instead of simply the length of the velocity vector.
    /// </remarks>
    public override float Speed {
        get { 
            return Speedometer == null ? _speed : Speedometer.Speed; 
        }
        set {
            throw new System.NotSupportedException ("Cannot set the speed directly on PassiveVehicle");
        }
    }

    /// <summary>
    /// Current vehicle velocity
    /// </summary>
    public override Vector3 Velocity {
        get {
            return Transform.forward * _speed;
        }
        set {
            throw new System.NotSupportedException ("Cannot set the velocity directly on PassiveCehicle");
        }
    }
}


