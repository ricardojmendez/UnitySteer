using UnityEngine;
using System.Collections;
using UnitySteer;
using UnitySteer.Vehicles;

public class PursuerBehavior : MonoBehaviour {

    MpPursuer pursuer;
    
    public WanderBehavior   wanderer;
    public bool             randomize;
    public float            MaxSpeed = 10;
    public float            MaxForce =  2;

	void Start () {
	    pursuer = new MpPursuer(transform, 1.0f, wanderer.Wanderer);
	    pursuer.MaxSpeed = MaxSpeed;
	    pursuer.MaxForce = MaxForce;
	    
	    if (randomize)
	    {
	        pursuer.randomizeStartingPositionAndHeading();
	    }
	    
	}
	
	// Update is called once per frame
	void Update () {
	    pursuer.Update(Time.deltaTime);
	}
}
