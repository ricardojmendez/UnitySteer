using UnityEngine;


/// <summary>
/// Vehicle subclass that acts as an interface to CharacterMotor.
/// </summary>
/// <remarks>
/// IMPORTANT:You'll need to have imported the Character Controllers 
/// from Standard Assets for it to build, or you can just exclude it
/// from your project as it is not a key component.
/// </remarks>
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


