using UnityEngine;
using UnitySteer.Helpers;

[AddComponentMenu("UnitySteer/Steer/Steering")]
public class Steering : MonoBehaviour, ITick {	
	public const string STEERING_MESSAGE = "Steering";
	public const string ACTION_RETRY = "retry";
	
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
	Tick _tick;
	
	[SerializeField]
	float _weight = 5;
	#endregion
	
	
	#region Public properties
	/// <summary>
	/// The force vector calculated by this steering behavior
	/// </summary>
	public Vector3 Force
	{
		get
		{
			if (Tick.ShouldTick())
			{
				_force = CalculateForce();
			}
			if (_force != Vector3.zero)
			{
				ReportedArrival = false;
			}
			else if (!ReportedArrival)
			{
				ReportedArrival = true;
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
	


	/// <summary>
	/// Steering event handler for arrival notification
	/// </summary>
	public SteeringEventHandler<Vehicle> OnArrival { get; set; }
	
	/// <summary>
	/// Have we reported that we stopped moving?
	/// </summary>
	public bool ReportedArrival { get; protected set; }	
	
	
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
	/// Tick information
	/// </summary>
	public Tick Tick 
	{
		get 
		{
			return _tick;
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