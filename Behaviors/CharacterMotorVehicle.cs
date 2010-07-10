using UnityEngine;


/// <summary>
/// Vehicle subclass that acts as an interface to CharacterMotor.  You'll need
/// to have imported the Character Controllers from Standard Assets for it to
/// build.
/// </summary>
[RequireComponent(typeof(CharacterMotor))]
public class CharacterMotorVehicle: Vehicle
{
	#region Private fields
	CharacterMotor motor;
	#endregion 
	
	#region Methods
	void Awake()
	{
		motor = gameObject.GetComponent<CharacterMotor>();
		this.MaxSpeed = motor.movement.maxForwardSpeed;
	}
	
	void Update()
	{
		this.Speed = motor.movement.velocity.magnitude;
	}
	#endregion
}


