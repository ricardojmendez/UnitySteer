using UnityEngine;
using System.Collections;
using UnitySteer;
using UnitySteer.Vehicles;

[RequireComponent(typeof(SphereCollider))]
public class BoidBehaviour : VehicleBehaviour, IRadarReceiver {
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
    
    public float randomizeStart = 15f;
    
	// Use this for initialization
	void Start()
	{
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
	    boid.Update(Time.time, Time.deltaTime);
	}



	public Obstacle GetObstacle( GameObject gameObject )
	{
		Obstacle obstacle;
		int id = gameObject.GetInstanceID();
		Component[] colliders;
		float radius = 0.0f, currentRadius;
		
		if( !obstacles.ContainsKey( id ) )
		{
			colliders = gameObject.GetComponentsInChildren( typeof( Collider ) );
			
			if( colliders == null )
			{
				Debug.LogError( "Obstacle '" + gameObject.name + "' has no colliders" );
				return null;
			}
			
			foreach( Collider collider in colliders )
			{
				if( collider.isTrigger )
				{
					continue;
				}
				
				currentRadius = Mathf.Abs( ( gameObject.transform.position - ( collider.transform.position + collider.bounds.center ) ).x ) + collider.bounds.extents.x;
				currentRadius *= gameObject.transform.localScale.x;
				//currentRadius = gameObject.transform.localScale.x / 2.0f;
				
				if( currentRadius > radius )
				{
					radius = currentRadius;
				}
			}
			obstacle = new SphericalObstacle( radius, gameObject.transform.position );
		}
		else
		{
			obstacle = obstacles[ id ] as Obstacle;
		}
		
		return obstacle;
	}
	
	
	
	public void OnRadarEnter( Collider other, Radar sender )
	{
		BoidBehaviour boidBehaviour;
		Obstacle obstacle;

		if( ( 1 << other.gameObject.layer & ObstacleLayer ) > 0 )
		{
			obstacle = GetObstacle( other.gameObject );
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
			obstacle = GetObstacle( other.gameObject );
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
