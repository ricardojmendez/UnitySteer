using UnityEngine;
using System.Collections;
using OpenSteer;
using OpenSteer.Vehicles;

public class WanderBehavior : MonoBehaviour {

    public MpWanderer wanderer = new MpWanderer();

	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	    wanderer.update(Time.time, Time.deltaTime);
	    transform.position = wanderer.Position;
	}
}
