using UnityEngine;
using System.Collections;
using OpenSteer;
using OpenSteer.Vehicles;

public class BoidBehavior : MonoBehaviour {
    static BruteForceProximityDatabase pd;
    
    
    Boid boid;

	// Use this for initialization
	void Start () {
	    if (pd == null)
	        pd = new BruteForceProximityDatabase();
	    boid = new Boid(pd);
	    // vehicle.randomizeHeadingOnXZPlane();
	}
	
	// Update is called once per frame
	void Update () {
	    boid.update(Time.time, Time.deltaTime);
	    transform.position = boid.Position;
	}
}
