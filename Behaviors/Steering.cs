using UnityEngine;
using UnitySteer.Helpers;
using TickedPriorityQueue;

namespace UnitySteer.Base
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
					OnStartMoving(this);
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
					OnArrival(this);
					// It's possible that any of the OnArrival handlers indicated we should recalculate
					// our forces.
					if (ShouldRetryForce)
					{
						_force = CalculateForce();
						ShouldRetryForce = false;
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
	public System.Action<Steering> OnArrival = delegate{};
	
	/// <summary>
	/// Steering event handler for arrival notification
	/// </summary>
	public System.Action<Steering> OnStartMoving { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether this <see cref="UnitySteer.Base.Steering"/> should recalculate its force.
	/// </summary>
	/// <value><c>true</c> if recalculate force; otherwise, <c>false</c>.</value>
	public bool ShouldRetryForce { get; protected set; }
	
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
	protected virtual void Awake()
	{
		_vehicle = this.GetComponent<Vehicle>();
		ReportedArrival = true; // Default to true to avoid unnecessary notifications
	}
	
	protected virtual void Start()
	{
	}
	
	/// <summary>
	/// Calculates the force desired by this behavior
	/// </summary>
	/// <returns>
	/// A vector with the desired force <see cref="Vector3"/>
	/// </returns>
	protected abstract Vector3 CalculateForce();
	

	#endregion
}

}