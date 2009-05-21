using UnityEngine;
using System.Collections;
using UnitySteer;
using UnitySteer.Vehicles;

[RequireComponent(typeof(SphereCollider))]
public class BoidBehavior : MonoBehaviour, IVehicleBehaviour, IRadarReceiver {
    static Hashtable obstacles;

    Boid boid;
    
    public LayerMask ObstacleLayer;
    
    public bool MovesVertically = true;
    
    public float separationRadius =  5.0f;
    public float separationAngle  = -0.707f;
    public float separationWeight =  12.0f;

    public float alignmentRadius = 7.5f;
    public float alignmentAngle  = 0.7f;
    public float alignmentWeight = 8.0f;

    public float cohesionRadius = 9.0f;
    public float cohesionAngle  = -0.15f;
    public float cohesionWeight = 8.0f;    
    
    public float maxSpeed =  3f;
    public float maxForce = 15f;
    
	// Use this for initialization
	void Start () {
	    if (obstacles == null)
	        obstacles = new Hashtable();
	    
		if( rigidbody == null )
		{
			// Debug.Log( "Boid: Transform" );
	    	boid = new Boid(transform, 1.0f, MovesVertically);
		}
		else
		{
			// Debug.Log( "Boid: Rigidbody" );
	    	boid = new Boid(rigidbody, MovesVertically);
		}
	    // vehicle.randomizeHeadingOnXZPlane();
	    
	    boid.separationRadius = separationRadius;
        boid.separationAngle  = separationAngle;
        boid.separationWeight = separationWeight;

        boid.alignmentRadius = alignmentRadius;
        boid.alignmentAngle  = alignmentAngle;
        boid.alignmentWeight = alignmentWeight;

        boid.cohesionRadius = cohesionRadius;
        boid.cohesionAngle  = cohesionAngle;
        boid.cohesionWeight = cohesionWeight;
        
        boid.MaxSpeed = maxSpeed;
        boid.MaxForce = maxForce;
        
        boid.Obstacles = new ArrayList();
	}
	
	public Vehicle Vehicle
	{
		get
		{
			return boid;
		}
	}
	
	// Update is called once per frame
	void Update () {
	    boid.update(Time.time, Time.deltaTime);
	}
	
	void OnTriggerEnter(Collider collider)
	{
	    HandleVisibility(collider.gameObject, true);
	}

	void OnTriggerExit(Collider collider)
	{
	    HandleVisibility(collider.gameObject, false);
	}
	
	void HandleVisibility(GameObject other, bool visible)
	{
	    //Debug.Log(other.layer + " " + ObstacleLayer.value);
        if ((1 << other.layer & ObstacleLayer) > 0)
        {
            SphericalObstacle o;
            int id = other.GetInstanceID();
            if (!obstacles.ContainsKey(id))
            {
                o = CreateObstacle(other.transform);
                obstacles[id] = o;
            }
            else
                o = obstacles[id] as SphericalObstacle;
            if (visible)
                boid.Obstacles.Add(o);
            else
                boid.Obstacles.Remove(o);
        }
	}
	
	SphericalObstacle CreateObstacle(Transform obstacleTransform)
	{
	    SphericalObstacle obstacle = new SphericalObstacle(obstacleTransform.localScale.x / 2,
	        obstacleTransform.position);
	    return obstacle;
	}
	
	public void OnRadarEnter( Collider other, Radar sender )
	{
		IVehicleBehaviour vehicleBehaviour;
		
		vehicleBehaviour = other.GetComponent( typeof( IVehicleBehaviour ) ) as IVehicleBehaviour;
		if( vehicleBehaviour != null )
		{
        	boid.Neighbors.Add( vehicleBehaviour.Vehicle );
		}
	}
	
	public void OnRadarExit( Collider other, Radar sender )
	{
		IVehicleBehaviour vehicleBehaviour;
		
		vehicleBehaviour = other.GetComponent( typeof( IVehicleBehaviour ) ) as IVehicleBehaviour;
		if( vehicleBehaviour != null )
		{
        	boid.Neighbors.Remove( vehicleBehaviour.Vehicle );
		}
	}
	
	public void OnRadarStay( Collider other, Radar sender )
	{
		
	}
}
