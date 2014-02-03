//#define DEBUG_DRAWNEIGHBORS
using UnityEngine;
using UnitySteer;
using UnitySteer.Helpers;

namespace UnitySteer.Base
{

/// <summary>
/// Base class for behaviors which steer a vehicle in relation to detected neighbors
/// </summary>
/// <remarks>
/// Sample values to user for flocking boids (angles are in degrees):
/// 
/// public float separationRadius =   5;
/// public float separationAngle  = 135;
/// public float separationWeight =  12;
/// 
/// public float alignmentRadius =    7.5f;
/// public float alignmentAngle  =   45;
/// public float alignmentWeight =    8;
/// 
/// public float cohesionRadius  =    9;
/// public float cohesionAngle   =   99;
/// public float cohesionWeight  =    8;   
/// </remarks>
public abstract class SteerForNeighbors : Steering
{
	
	#region Methods
	protected sealed override Vector3 CalculateForce ()
	{
		// Return an empty value. Everything will be calculated
		// by SteerForNeighborGroup.
		throw new System.NotImplementedException("SteerForNeighbors.CalculateForce should never be called directly");
	}
	
	public abstract Vector3 CalculateNeighborContribution(Vehicle other);
	#endregion	
}

}
