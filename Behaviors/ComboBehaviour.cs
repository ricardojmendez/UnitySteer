using UnityEngine;
using System.Collections;
using UnitySteer;
using UnitySteer.Vehicles;

public class ComboBehaviour : MonoBehaviour, IVehicleBehaviour, IRadarReceiver
{
	
	public float steerToAvoidNeighbours, steerToAvoidObstacles, steerToStayOnPath, steerForTargetSpeed;
	private float steerForPursuit;
	
	private ComboVehicle vehicle;
	private Vehicle target;
	
	
	
	public void Awake()
	{
		vehicle = new ComboVehicle( transform, 1.0f );
		
		SteerToAvoidNeighbours = steerToAvoidNeighbours;
		SteerToAvoidObstacles = steerToAvoidObstacles;
		SteerToStayOnPath = steerToStayOnPath;
		SteerForTargetSpeed = steerForTargetSpeed;
	}
	
	
	
	public Vehicle Vehicle
	{
		get
		{
			return vehicle;
		}
	}
	
	
	
	public void OnRadarEnter( Collider other, Radar sender )
	{
		IVehicleBehaviour vehicleBehaviour;
		
		if( steerToAvoidNeighbours == 0.0f )
		{
			return;
		}
		
		vehicleBehaviour = other.GetComponent( typeof( IVehicleBehaviour ) ) as IVehicleBehaviour;
		if( vehicleBehaviour != null )
		{
        	vehicle.Neighbors.Add( vehicleBehaviour.Vehicle );
		}
	}
	
	
	
	public void OnRadarExit( Collider other, Radar sender )
	{
		IVehicleBehaviour vehicleBehaviour;
		
		if( steerToAvoidNeighbours == 0.0f )
		{
			return;
		}
		
		vehicleBehaviour = other.GetComponent( typeof( IVehicleBehaviour ) ) as IVehicleBehaviour;
		if( vehicleBehaviour != null )
		{
        	vehicle.Neighbors.Remove( vehicleBehaviour.Vehicle );
		}
	}
	
	
	
	public void OnRadarStay( Collider other, Radar sender )
	{
		
	}
	
	
	
	public float SteerToAvoidNeighbours
	{
		get
		{
			return steerToAvoidNeighbours;
		}
		set
		{
			steerToAvoidNeighbours = value;
			vehicle.SteerToAvoidNeighboursWeight = steerToAvoidNeighbours;
		}
	}
	
	
	
	public float SteerToAvoidObstacles
	{
		get
		{
			return steerToAvoidObstacles;
		}
		set
		{
			steerToAvoidObstacles = value;
			vehicle.SteerToAvoidObstaclesWeight = steerToAvoidObstacles;
		}
	}
	
	
	
	public float SteerToStayOnPath
	{
		get
		{
			return steerToStayOnPath;
		}
		set
		{
			steerToStayOnPath = value;
			vehicle.SteerToStayOnPathWeight = steerToStayOnPath;
		}
	}
	
	
	
	public float SteerForPursuit
	{
		get
		{
			return steerForPursuit;
		}
		set
		{
			steerToStayOnPath = value;
			vehicle.SteerForPursuitWeight = steerForPursuit;
		}
	}
	
	
	
	public float SteerForTargetSpeed
	{
		get
		{
			return steerForTargetSpeed;
		}
		set
		{
			steerForTargetSpeed = value;
			vehicle.SteerForTargetSpeedWeight = steerForTargetSpeed;
		}
	}



	public Vehicle Target
	{
		get
		{
			return target;
		}
	}



	public void SetTarget( Vehicle target, float steeringWeight )
	{
		this.target = target;
		SteerForPursuit = steeringWeight;
	}
	
	
	
	public void ClearTarget()
	{
		SetTarget( null, 0.0f );
	}



	public void Update()
	{
	    vehicle.Update( Time.time, Time.deltaTime );
	}
}