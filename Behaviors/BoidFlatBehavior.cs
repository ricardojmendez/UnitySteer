using UnityEngine;
using System.Collections;
using OpenSteer;
using OpenSteer.Vehicles;

public class BoidFlatBehavior : MonoBehaviour {
    static BruteForceProximityDatabase pd;
    
    
    BoidFlat boid;

	// Use this for initialization
	void Start () {
	    if (pd == null)
	        pd = new BruteForceProximityDatabase();
	    boid = new BoidFlat(pd);
	    // vehicle.randomizeHeadingOnXZPlane();
	}
	
	// Update is called once per frame
	void Update () {
	    boid.update(Time.time, Time.deltaTime);
	    transform.position = boid.Position;
	    Vector3 f = boid.forward();
	    f.y = transform.forward.y;
	    transform.forward = f;
	}
}
