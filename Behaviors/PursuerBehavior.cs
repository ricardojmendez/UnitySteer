using UnityEngine;
using System.Collections;
using UnitySteer;
using UnitySteer.Vehicles;

public class PursuerBehavior : MonoBehaviour {

    MpPursuer pursuer;
    
    public WanderBehavior wanderer;
    public bool randomize;

	void Start () {
	    pursuer = new MpPursuer( transform, 1.0f, wanderer.wanderer);
	    if (randomize)
	    {
	        pursuer.randomizeStartingPositionAndHeading();
	    }
	    
	}
	
	// Update is called once per frame
	void Update () {
	    pursuer.Position = transform.position;
	    pursuer.update(Time.time, Time.deltaTime);
	    transform.position = pursuer.Position;
	}
}
