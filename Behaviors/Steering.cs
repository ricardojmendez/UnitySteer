using UnityEngine;

namespace UnitySteer.Behaviors
{

/// <summary>
/// Base Steering class from which other steering behaviors derive
/// </summary>
/// <remarks>
/// This is an abstract class because it does not provide any steering
/// itself.  It should be subclassed for your particular steering needs.
/// </remarks>
public abstract class Steering : MonoBehaviour {	
	#region Private fields
	/// <summary>
	/// Last force calculated
	/// </summary>
	Vector3 _force = Vector3.zero;
	
	/// <summary>
	/// Cached vehicle
	/// </summary>
	Vehicle _vehicle;
	
	[SerializeField]
	float _weight = 1;
	
	#endregion
	
	#region Public properties
	/// <summary>
	/// Calculates the force provided by this steering behavior and raises any
    /// arrival/start events.
	/// </summary>
    /// <remarks>
    /// Besides calculating the desired force, it will also notify the vehicle
    /// of when it started/stopped providing force, via the OnArrival and
    /// OnStartMoving events.  If an OnArrival even is raised, the receiving
    /// object can set the ShouldRetryForce property to TRUE to force the vehicle
    /// recalculating the force once.
    /// </remarks>
	public Vector3 Force
	{
		get
		{
			_force = CalculateForce();
			if (_force != Vector3.zero)
			{
				if (!ReportedMove && OnStartMoving != null)
				{
					OnStartMoving(this);
				}
				ReportedArrival = false;
				ReportedMove = true;
			}
			else if (!ReportedArrival)
			{
				if (OnArrival != null)
				{
					OnArrival(this);
					// It's possible that any of the OnArrival handlers indicated we should recalculate
					// our forces.
					if (ShouldRetryForce)
					{
						_force = CalculateForce();
						ShouldRetryForce = false;
					}
				}
				if (_force == Vector3.zero)
				{
					ReportedArrival = true;
					ReportedMove = false;
				}
			}
			return _force;
		}
	}
		
	public virtual bool IsPostProcess 
	{ 
		get { return false; }
	}
	


	/// <summary>
	/// Steering event handler for arrival notification
	/// </summary>
	public System.Action<Steering> OnArrival = delegate{};
	
	/// <summary>
	/// Steering event handler for arrival notification
	/// </summary>
	public System.Action<Steering> OnStartMoving { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether this Steering should recalculate 
    /// its force.
	/// </summary>
	/// <value><c>true</c> if recalculate force; otherwise, <c>false</c>.</value>
    /// <remarks>
    /// This property is checked once after the steering behavior has raised an
    /// OnArrival event, and if it is true, the force is then recalculated. This
    /// is useful in cases of vehicles which do not recalculate their forces
    /// even frame, since we may want to provide some continuity of movement in
    /// some cases (for instance, when moving from one waypoint to another) 
    /// instead of having the vehicle stop at a point until the next time that
    /// the Force is explicitly queried.
    /// </remarks>
	public bool ShouldRetryForce { get; set; }
	
	/// <summary>
	/// Have we reported that we stopped moving?
	/// </summary>
	public bool ReportedArrival { get; protected set; }
	
	/// <summary>
	/// Have we reported that we began moving?
	/// </summary>
	public bool ReportedMove { get; protected set; }
	
	
	/// <summary>
	/// Force vector modified by the assigned weight 
	/// </summary>
	public Vector3 WeighedForce
	{
		get { return Force * _weight; }
	}
	
	/// <summary>
	/// Vehicle that this behavior will influence
	/// </summary>
	public Vehicle Vehicle 
    {
		get { return _vehicle; }
	}
	
	/// <summary>
	/// Weight assigned to this steering behavior
	/// </summary>
    /// <remarks>
    /// The weight is used by WeighedForce to return a modified force value to
    /// the vehicle, which will then blend all weighed forces from its steerings
    /// to calculate the desired force.
    /// </remarks>
	public float Weight 
    {
		get { return _weight; }
		set { _weight = value; }
	}
	#endregion
	
	#region Methods
	protected virtual void Awake()
	{
		_vehicle = GetComponent<Vehicle>();
		ReportedArrival = true; // Default to true to avoid unnecessary notifications
	}
	
	protected virtual void Start()
	{
	}
	
	/// <summary>
	/// Calculates the force supplied by this behavior
	/// </summary>
	/// <returns>
	/// A vector with the supplied force <see cref="Vector3"/>
	/// </returns>
	protected abstract Vector3 CalculateForce();
	

	#endregion
}

}