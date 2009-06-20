using UnityEngine;
using System.Collections;
using UnitySteer;
using UnitySteer.Vehicles;

public class WanderBehavior : MonoBehaviour {

    public MpWanderer wanderer;
    public bool MovesVertically;

	void Start()
	{
		wanderer = new MpWanderer( transform, 1.0f, MovesVertically );
	}
	
	// Update is called once per frame
	void Update () {
	    wanderer.Position = transform.position;
	    wanderer.Update(Time.time, Time.deltaTime);
	    transform.position = wanderer.Position;
	}
}
