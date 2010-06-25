using UnityEngine;
using System.Collections;
using UnitySteer;
using UnitySteer.Vehicles;

public class WanderBehavior : VehicleBehaviour {

	private MpWanderer	wanderer;
	
	
	public bool			MovesVertically;
	public float		MaxSpeed;
	public float		MaxForce;
	public float		Mass;
	public Transform	Tether;
	public float		MaxDistance;
	public float		Radius = 1;
	
	public MpWanderer Wanderer
	{
		get
		{
			return wanderer;
		}
	}
	
	public override SteeringVehicle Vehicle 
	{
		get { return wanderer; }
	}
	

	protected void Start()
	{
		wanderer = new MpWanderer( transform, Mass, MovesVertically );
		wanderer.MaxSpeed = MaxSpeed;
		wanderer.MaxForce = MaxForce;
		wanderer.MaxDistance = MaxDistance;
		wanderer.Tether = Tether;
		wanderer.Radius = Radius;
	}
	
	// Update is called once per frame
	protected void Update () {
		wanderer.Update(Time.deltaTime);
	}
}
