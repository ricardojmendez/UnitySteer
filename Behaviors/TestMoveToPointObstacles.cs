//#define DEBUG
using UnityEngine;
using System.Collections;
using UnitySteer;
using UnitySteer.Vehicles;

/*
    Simple class created to test vehicle neighbor and obstacle avoidance
 */
public class TestMoveToPointObstacles : VehicleBehaviour, IRadarReceiver {
    
    private MovesToPoint vehicle;
    
    public LayerMask ObstacleLayer;
    public LayerMask NeighborLayer;
    public Transform target;
    public float     maxSpeed =  3;
    public float     maxForce = 15;
    public float     radius   =  2;
    public float     avoidDegrees = 45;
    
    
    public override Vehicle Vehicle
    {
        get { return vehicle; }
    }
    

	// Use this for initialization
	void Start () {
	    vehicle = new MovesToPoint(this.rigidbody, target.position, radius);
	    vehicle.MaxSpeed = maxSpeed;
	    vehicle.MaxForce = maxForce;
	    vehicle.AvoidDeg = avoidDegrees;
	}
	
	// Update is called once per frame
	void Update () {
	    vehicle.Update(Time.deltaTime);
	}
	
	void OnDrawGizmos() {
	    Gizmos.color = Color.gray;
	    Gizmos.DrawWireSphere(transform.position, radius);
	}
	
	
	#region Radar methods
	public void OnRadarEnter( Collider other, Radar sender )
	{
	    HandleVisibility(other, true);
	}
	
	public void OnRadarExit( Collider other, Radar sender )
	{
	    HandleVisibility(other, false);
	}
	
	public void OnRadarStay( Collider other, Radar sender )
	{
		
	}
	
	public void HandleVisibility(Collider other, bool visible)
	{
	    ArrayList list = null;
	    System.Object obj = null;
		int layerMask = 1 << other.gameObject.layer;
		if((layerMask & ObstacleLayer) != 0)
		{
    			Obstacle obstacle;
			obstacle = SphericalObstacle.GetObstacle( other.gameObject );
			if( obstacle != null )
			{
			    obj  = obstacle;
			    list = vehicle.Obstacles;
			}
		}
		else if ((layerMask & NeighborLayer) != 0)
		{
		    // Objects considered neighbors are expected to have a VehicleBehavior component
		    VehicleBehaviour s = other.gameObject.GetComponent(typeof(VehicleBehaviour)) as VehicleBehaviour;
		    if (s != null)
		    {
		        obj  = s.Vehicle;
		        list = Vehicle.Neighbors;
		    }
		}
		if (list != null)
		{
		    if (visible && !list.Contains(obj))
		    {
		        list.Add(obj);
		        #if DEBUG
		        Debug.Log(gameObject.name+" can see "+obj+" "+list.Count);
		        #endif
	        }
		    else if (!visible)
		    {
		        list.Remove(obj);
		        #if DEBUG
		        Debug.Log(gameObject.name+" no longer sees "+obj+" "+list.Count);
		        #endif
	        }
		}
	    
	}
	#endregion
}
