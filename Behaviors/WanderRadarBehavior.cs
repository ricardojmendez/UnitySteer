using UnityEngine;
using System.Collections;
using UnitySteer;
using UnitySteer.Vehicles;

public class WanderRadarBehavior : WanderBehavior, IRadarReceiver {

    static Hashtable obstacles;

    public LayerMask ObstacleLayer;
    
    protected void Start()
    {
        base.Start();
        if (obstacles == null)
        {
	        obstacles = new Hashtable();
        }
    }
    
    protected void Update()
    {
        base.Update();
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
			obstacles[id] = new SphericalObstacle( radius, gameObject.transform.position );
		}
		obstacle = obstacles[ id ] as Obstacle;
		
		return obstacle;
	}
	
	
	
	public void OnRadarEnter( Collider other, Radar sender )
	{
		Obstacle obstacle;

		if( ( 1 << other.gameObject.layer & ObstacleLayer ) > 0 )
		{
			obstacle = GetObstacle( other.gameObject );
			if( obstacle != null )
			{
				Wanderer.Obstacles.Add( obstacle );
			}
		}
	}
	
	
	
	public void OnRadarExit( Collider other, Radar sender )
	{
		Obstacle obstacle;
		
		if( ( 1 << other.gameObject.layer & ObstacleLayer ) > 0 )
		{
			obstacle = GetObstacle( other.gameObject );
			if( obstacle != null )
			{
				Wanderer.Obstacles.Remove( obstacle );
			}
		}
	}
	
	
	
	public void OnRadarStay( Collider other, Radar sender )
	{
		
	}
	
}
