using UnityEngine;
using UnitySteer.Helpers;
using TickedPriorityQueue;

/// <summary>
/// Base Steering class from which other steering behaviors derive
/// </summary>
/// <remarks>
/// This is an abstract class because it does not provide any steering
/// itself.  It should be subclassed for your particular steering needs.
/// </remarks>
public abstract class Steering : MonoBehaviour {	
	public static readonly string STEERING_MESSAGE = "Steering";
	public static readonly string ACTION_RETRY = "retry";
	
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
	/// The force vector calculated by this steering behavior
	/// </summary>
	public Vector3 Force
	{
		get
		{
			_force = CalculateForce();
			if (_force != Vector3.zero)
			{
				if (!ReportedMove && OnStartMoving != null)
				{
					OnStartMoving(new SteeringEvent<Vehicle>(this, "moving", Vehicle));
				}
				ReportedArrival = false;
				ReportedMove = true;
			}
			else if (!ReportedArrival)
			{
				ReportedArrival = true;
				ReportedMove = false;
				if (OnArrival != null)
				{
					var message = new SteeringEvent<Vehicle>(this, "arrived", Vehicle);
					OnArrival(message);
					if (message.Action == ACTION_RETRY)
					{
						_force = CalculateForce();
					}
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
	public SteeringEventHandler<Vehicle> OnArrival { get; set; }
	
	/// <summary>
	/// Steering event handler for arrival notification
	/// </summary>
	public SteeringEventHandler<Vehicle> OnStartMoving { get; set; }
	
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
		get {
			return Force * _weight;
		}
	}
	
	/// <summary>
	/// Vehicle that this behavior will influence
	/// </summary>
	public Vehicle Vehicle {
		get { return _vehicle; }
	}
	
	/// <summary>
	/// Weight assigned to this steering behavior
	/// </summary>
	public float Weight {
		get {
			return this._weight;
		}
		set {
			_weight = value;
		}
	}
	#endregion
	
	#region Methods
	protected void Start()
	{
		_vehicle = this.GetComponent<Vehicle>();
		ReportedArrival = true; // Default to true to avoid unnecessary notifications
	}
	
	/// <summary>
	/// Calculates the force desired by this behavior
	/// </summary>
	/// <returns>
	/// A vector with the desired force <see cref="Vector3"/>
	/// </returns>
	protected virtual Vector3 CalculateForce()
	{
		return Vector3.zero;
	}
	

	#endregion
}