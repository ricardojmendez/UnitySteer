using UnityEngine;
using System.Collections;
using UnitySteer;
using UnitySteer.Vehicles;

public class WanderBehavior : MonoBehaviour {

    private MpWanderer  wanderer;
    
    
    public bool         MovesVertically;
    public float        MaxSpeed;
    public float        MaxForce;
    public float        Mass;
    public Transform    Tether;
    public float        MaxDistance;
    
    public MpWanderer Wanderer
    {
        get
        {
            return wanderer;
        }
    }
    

	void Start()
	{
		wanderer = new MpWanderer( transform, Mass, MovesVertically );
	}
	
	// Update is called once per frame
	void Update () {
	    wanderer.Position = transform.position;
	    wanderer.Update(Time.deltaTime);
	    transform.position = wanderer.Position;
	    wanderer.MaxSpeed = MaxSpeed;
	    wanderer.MaxForce = MaxForce;
	    wanderer.MaxDistance = MaxDistance;
	    wanderer.Tether = Tether;
	}
}
