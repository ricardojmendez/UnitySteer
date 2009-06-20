using UnityEngine;
using System.Collections;
using UnitySteer;

namespace UnitySteer.Vehicles
{
    public class ComboVehicle : SimpleVehicle
    {
		private float steerToAvoidNeighborsWeight, steerToAvoidObstaclesWeight, steerToStayOnPathWeight, steerForPursuitWeight, steerForTargetSpeedWeight;
		private Pathway path;
		private Vector3 avoidNeighbors, avoidObstacles, stayOnPath, pursuit, targetSpeed;
		
		
		
		public ComboVehicle( Transform transform, float mass ) : base( transform, mass )
		{
			reset();
		}
		
		
		
		public ComboVehicle( Rigidbody rigidbody ) : base( rigidbody )
		{
			reset();
		}
		
		
		
        // reset state
        new void reset()
        {
            // reset the vehicle
            base.reset();
            // initial slow speed
            Speed = MaxSpeed * 0.3f;
        }

        public Vector3 AvoidNeighbors
        {
            get { return avoidNeighbors; }
        }
        
        public Vector3 AvoidObstacles
        {
            get { return avoidObstacles; }
        }
        
        public Vector3 StayOnPath
        {
            get { return stayOnPath; }
        }
        
        public Vector3 Pursuit
        {
            get { return pursuit; }
        }
        
        public Vector3 TargetSpeed
        {
            get { return targetSpeed; }
        }
		
		public float SteerToAvoidNeighborsWeight
		{
			get
			{
				return steerToAvoidNeighborsWeight;
			}
			set
			{
				steerToAvoidNeighborsWeight = value;
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
			avoidNeighbors = avoidObstacles = stayOnPath = pursuit = targetSpeed = Vector3.zero;

			if( steerToAvoidNeighborsWeight != 0.0f )
			{
				avoidNeighbors = steerToAvoidNeighbors( 0.2f, Neighbors ) * steerToAvoidNeighborsWeight;
				// TODO: Expose time as a variable
			}
			
			if( steerToAvoidObstaclesWeight != 0.0f )
			{
				avoidObstacles = steerToAvoidObstacles( 0.2f, Obstacles ) * steerToAvoidObstaclesWeight;
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
				targetSpeed = steerForTargetSpeed( MaxSpeed ) * steerForTargetSpeedWeight;
					// TODO: Expose target speed variable - dont use max speed
			}

            applySteeringForce( avoidNeighbors + avoidObstacles + stayOnPath + pursuit + targetSpeed, elapsedTime );
        }
	}
}
