using UnityEngine;
using System.Collections;
using OpenSteer;

namespace OpenSteer.Vehicles
{
    public class ComboVehicle : SimpleVehicle
    {
		private float steerToAvoidNeighboursWeight, steerToAvoidObstaclesWeight, steerToStayOnPathWeight, steerForPursuitWeight, steerForTargetSpeedWeight;
		private Pathway path;
		
		
		
		public ComboVehicle( Transform transform, float mass ) : base( transform, mass ){}
		public ComboVehicle( Rigidbody rigidbody ) : base( rigidbody ){}
		
		
		
		public float SteerToAvoidNeighboursWeight
		{
			get
			{
				return steerToAvoidNeighboursWeight;
			}
			set
			{
				steerToAvoidNeighboursWeight = value;
			}
		}
		
		
		
		public float SteerToAvoidObstaclesWeight
		{
			get
			{
				return steerToAvoidObstaclesWeight;
			}
			set
			{
				steerToAvoidObstaclesWeight = value;
			}
		}
		
		
		
		public float SteerToStayOnPathWeight
		{
			get
			{
				return steerToStayOnPathWeight;
			}
			set
			{
				steerToStayOnPathWeight = value;
			}
		}
		
		
		
		public float SteerForPursuitWeight
		{
			get
			{
				return steerForPursuitWeight;
			}
			set
			{
				steerForPursuitWeight = value;
			}
		}
		
		
		
		public float SteerForTargetSpeedWeight
		{
			get
			{
				return steerForTargetSpeedWeight;
			}
			set
			{
				steerForTargetSpeedWeight = value;
			}
		}



		public Pathway Path
		{
			get
			{
				return path;
			}
			set
			{
				path = value;
			}
		}



		public void SetPath( Pathway path, float weight )
		{
			Path = path;
			SteerToStayOnPathWeight = weight;
		}



        public void Update( float currentTime, float elapsedTime )
        {
			// TODO: Continue work here once we're shiny
            Vector3 avoidNeighbours, avoidObstacles, stayOnPath, pursuit, targetSpeed;

			avoidNeighbours = avoidObstacles = stayOnPath = pursuit = targetSpeed = Vector3.zero;

			if( steerToAvoidNeighboursWeight != 0.0f )
			{
//				avoidNeighbours = steerToAvoidNeighbours( float minTimeToCollision, ArrayList others ) * steerToAvoidNeighboursWeight;
			}
			
			if( steerToAvoidObstaclesWeight != 0.0f )
			{
//				avoidObstacles = steerToAvoidObstacles( float minTimeToCollision, ArrayList obstacles ) * steerToAvoidObstaclesWeight;
			}
			
			if( steerToStayOnPathWeight != 0.0f )
			{
//				stayOnPath = steerToStayOnPath( float predictionTime, Pathway path ) * steerToStayOnPathWeight;
			}
			
			if( steerForPursuitWeight != 0.0f )
			{
//				pursuit = steerForPursuit( Vehicle quarry ) * steerForPursuitWeight;
			}
			
			if( steerForTargetSpeedWeight != 0.0f )
			{
//				targetSpeed = steerForTargetSpeed( float targetSpeed ) * steerForTargetSpeedWeight;
			}

            applySteeringForce( avoidNeighbours + avoidObstacles + stayOnPath + pursuit + targetSpeed, elapsedTime );
        }
	}
}
