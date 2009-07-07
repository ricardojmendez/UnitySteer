using UnityEngine;
using System.Collections;
using UnitySteer;
using UnitySteer.Vehicles;

public class BoidBehaviour : VehicleBehaviour, IRadarReceiver {
    Boid boid;
    
    public LayerMask ObstacleLayer;
    
    public bool MovesVertically = true;
    
    public float separationRadius =   5;
    public float separationAngle  = 135;
    public float separationWeight =  12;

    public float alignmentRadius =    7.5f;
    public float alignmentAngle  =   45;
    public float alignmentWeight =    8;

    public float cohesionRadius  =    9;
    public float cohesionAngle   =   99;
    public float cohesionWeight  =    8;    
    
    public float maxSpeed =  3f;
    public float maxForce = 15f;
    
    public float radius = 0.5f;
    
    public float randomizeStart = 15f;
    
	// Use this for initialization
	void Start()
	{
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
        boid.SeparationDeg    = separationAngle;
        boid.separationWeight = separationWeight;

        boid.alignmentRadius = alignmentRadius;
        boid.AlignmentDeg    = alignmentAngle;
        boid.alignmentWeight = alignmentWeight;

        boid.cohesionRadius = cohesionRadius;
        boid.CohesionDeg    = cohesionAngle;
        boid.cohesionWeight = cohesionWeight;
        
        boid.MaxSpeed = maxSpeed;
        boid.MaxForce = maxForce;
        boid.Radius   = radius;

        if (randomizeStart > 0)
            boid.Randomize(randomizeStart);
	}
	
	
	
	public override Vehicle Vehicle
	{
		get
		{
			return boid;
		}
	}
	
	
	// Update is called once per frame
	void Update () {
	    boid.Update(Time.deltaTime);
	}

	public void OnRadarEnter( Collider other, Radar sender )
	{
		BoidBehaviour boidBehaviour;
		Obstacle obstacle;

		if( ( 1 << other.gameObject.layer & ObstacleLayer ) > 0 )
		{
			obstacle = SphericalObstacle.GetObstacle( other.gameObject );
			if( obstacle != null )
			{
				boid.Obstacles.Add( obstacle );
			}
		}
		else
		{		
			boidBehaviour = other.GetComponent( typeof( BoidBehaviour ) ) as BoidBehaviour;
			if( boidBehaviour != null )
			{
        		boid.Neighbors.Add( boidBehaviour.Vehicle );
			}
		}
	}
	
	
	
	public void OnRadarExit( Collider other, Radar sender )
	{
		BoidBehaviour boidBehaviour;
		Obstacle obstacle;
		
		if( ( 1 << other.gameObject.layer & ObstacleLayer ) > 0 )
		{
			obstacle = SphericalObstacle.GetObstacle( other.gameObject );
			if( obstacle != null )
			{
				boid.Obstacles.Remove( obstacle );
			}
		}
		else
		{
			boidBehaviour = other.GetComponent( typeof( BoidBehaviour ) ) as BoidBehaviour;
			if( boidBehaviour != null )
			{
        		boid.Neighbors.Remove( boidBehaviour.Vehicle );
			}
		}
	}
	
	
	
	public void OnRadarStay( Collider other, Radar sender )
	{
		
	}
}
