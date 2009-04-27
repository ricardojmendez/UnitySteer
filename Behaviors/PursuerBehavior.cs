using UnityEngine;
using System.Collections;
using OpenSteer;
using OpenSteer.Vehicles;

public class PursuerBehavior : MonoBehaviour {

    MpPursuer pursuer;
    
    public WanderBehavior wanderer;

	void Start () {
	    pursuer = new MpPursuer(wanderer.wanderer);
	}
	
	// Update is called once per frame
	void Update () {
	    pursuer.update(Time.time, Time.deltaTime);
	    transform.position = pursuer.Position;
	}
}
