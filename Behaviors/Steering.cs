using UnityEngine;
using UnitySteer.Helpers;
using TickedPriorityQueue;

public class Steering : MonoBehaviour {	
	public const string STEERING_MESSAGE = "Steering";
	public const string ACTION_RETRY = "retry";
	
	#region Private fields
	/// <summary>
	/// Last force calculated
	/// </summary>
	Vector3 _force = Vector3.zero;
	
	[SerializeField]
	float _tickLength = 0.1f;
	
	TickedObject _tickedObject;
	UnityTickedQueue _steeringQueue;

	
	/// <summary>
	/// Cached vehicle
	/// </summary>
	Vehicle _vehicle;
	
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
	
	protected virtual string QueueName 
	{
		get { return "Steering"; }
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
	
	void OnEnable()
	{
		_tickedObject = new TickedObject(OnUpdateSteering);
		_tickedObject.TickLength = _tickLength;
		_steeringQueue = UnityTickedQueue.GetInstance(QueueName);
		_steeringQueue.Add(_tickedObject);
	}
	
	void OnDisable()
	{
		if (_steeringQueue != null)
		{
			_steeringQueue.Remove(_tickedObject);
		}
	}
	
	protected void OnUpdateSteering(object obj)
	{
		_force = CalculateForce();
	}

	#endregion
}