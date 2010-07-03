using System.Collections.Generic;
using UnityEngine;
using UnitySteer;
using UnitySteer.Helpers;


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
public class SteerForNeighbors : Steering
{
	#region Private properties
	[SerializeField]
	float _radius = 7.5f;
	[SerializeField]
	float _angleCos = 0.7f;
	#endregion
	
	
	#region Public properties
	/// <summary>
	/// Cosine of the maximum angle
	/// </summary>
	/// <remarks>All boid-like behaviors have an angle that helps limit them.
	/// We store the cosine of the angle for faster calculations</remarks>
	public float AngleCos {
		get {
			return this._angleCos;
		}
		set {
			_angleCos = Mathf.Clamp(value, -1.0f, 1.0f);
		}
	}
	
	/// <summary>
	/// Degree accessor for the angle
	/// </summary>
	/// <remarks>The cosine is actually used in calculations for performance reasons</remarks>
    public float AngleDeg
    {
        get
        {
            return OpenSteerUtility.DegreesFromCos(_angleCos);;
        }
        set
        {
            _angleCos = OpenSteerUtility.CosFromDegrees(value);
        }
    }	

	public float Radius {
		get {
			return this._radius;
		}
		set {
			_radius = value;
		}
	}	
	#endregion	
}
