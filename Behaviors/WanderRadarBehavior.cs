using UnityEngine;
using System.Collections;
using UnitySteer;
using UnitySteer.Vehicles;

public class WanderRadarBehavior : WanderBehavior, IRadarReceiver {
    public LayerMask ObstacleLayer;
    
    protected void Start()
    {
        base.Start();
    }
    
    protected void Update()
    {
        base.Update();
    }

		
	public void OnRadarEnter( Collider other, Radar sender )
	{
		Obstacle obstacle;

		if( ( 1 << other.gameObject.layer & ObstacleLayer ) > 0 )
		{
			obstacle = SphericalObstacle.GetObstacle( other.gameObject );
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
			obstacle = SphericalObstacle.GetObstacle( other.gameObject );
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
