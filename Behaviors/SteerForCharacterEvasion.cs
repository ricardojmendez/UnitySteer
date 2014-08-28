using UnityEngine;

namespace UnitySteer.Behaviors
{
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
        var predictionTime = ((roughTime > _predictionTime)
            ? _predictionTime
            : roughTime);

        var target = _menace.transform.position + (_menace.velocity * predictionTime);

        // This was the totality of SteerToFlee
        var desiredVelocity = Vehicle.Position - target;
        return desiredVelocity - Vehicle.Velocity;
    }
}
}