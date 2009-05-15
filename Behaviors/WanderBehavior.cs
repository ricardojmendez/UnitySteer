using UnityEngine;
using System.Collections;
using OpenSteer;
using OpenSteer.Vehicles;

public class WanderBehavior : MonoBehaviour {

    public MpWanderer wanderer;

	void Start()
	{
		wanderer = new MpWanderer( transform, 1.0f );
	}
	
	// Update is called once per frame
	void Update () {
	    wanderer.Position = transform.position;
	    wanderer.update(Time.time, Time.deltaTime);
	    transform.position = wanderer.Position;
	}
}
