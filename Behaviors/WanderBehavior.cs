using UnityEngine;
using System.Collections;
using UnitySteer;
using UnitySteer.Vehicles;

public class WanderBehavior : MonoBehaviour {

    public MpWanderer   wanderer;
    public bool         MovesVertically;
    public float        MaxSpeed;
    public float        MaxForce;
    public float        Mass;

	void Start()
	{
		wanderer = new MpWanderer( transform, Mass, MovesVertically );
	}
	
	// Update is called once per frame
	void Update () {
	    wanderer.Position = transform.position;
	    wanderer.Update(Time.time, Time.deltaTime);
	    transform.position = wanderer.Position;
	    wanderer.MaxSpeed = MaxSpeed;
	    wanderer.MaxForce = MaxForce;
	}
}
