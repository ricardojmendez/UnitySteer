#define DEBUG_DRAWNEIGHBORS
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
	float _minRadius = 3f;
	[SerializeField]
	float _maxRadius = 7.5f;
	[SerializeField]
	float _angleCos = 0.7f;	
	[SerializeField]
	LayerMask _layersChecked;
	
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
	
	/// <summary>
	/// Indicates the vehicles on which layers are evaluated on this behavior
	/// </summary>	
	public LayerMask LayersChecked {
		get {
			return this._layersChecked;
		}
		set {
			_layersChecked = value;
		}
	}

	/// <summary>
	/// Minimum radius in which another vehicle is definitely considered in the neighborhood
	/// </summary>
	public float MinRadius {
		get {
			return this._minRadius;
		}
		set {
			_minRadius = value;
		}
	}	
	
	/// <summary>
	/// Maximum neighborhood radius
	/// </summary>
	public float MaxRadius {
		get {
			return this._maxRadius;
		}
		set {
			_maxRadius = value;
		}
	}		
	#endregion	
	
	
	#region Methods
	protected override Vector3 CalculateForce ()
	{
		// steering accumulator and count of neighbors, both initially zero
		Vector3 steering = Vector3.zero;
		int neighbors = 0;
		
		// for each of the other vehicles...
		for (int i = 0; i < Vehicle.Radar.Vehicles.Count; i++)
		{
			Vehicle other = Vehicle.Radar.Vehicles[i];
			if ((1 << other.gameObject.layer & LayersChecked) != 0 &&
				Vehicle.IsInNeighborhood (other, MinRadius, MaxRadius, AngleCos)) 
			{
				#if DEBUG_DRAWNEIGHBORS
				Debug.DrawLine(Vehicle.Position, other.Position, Color.magenta);
				#endif
				steering += CalculateNeighborContribution(other);				
				neighbors++;
			}
		}

		// divide by neighbors, then normalize to pure direction
		if (neighbors > 0) {
			steering = (steering / (float)neighbors);
			steering.Normalize();
		}
		
		return steering;
	}
	
	protected virtual Vector3 CalculateNeighborContribution(Vehicle other)
	{
		return Vector3.zero;
	}
	#endregion
	
}
