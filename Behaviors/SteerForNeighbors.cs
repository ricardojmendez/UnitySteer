using UnityEngine;

namespace UnitySteer.Behaviors
{

/// <summary>
/// Base class for behaviors which steer a vehicle in relation to detected neighbors.
/// It does not return a force directly on CalculateForce, but instead just
/// returns a neighbor contribution when queried by SteerForNeighborGroup.
/// </summary>
public abstract class SteerForNeighbors : Steering
{
	
	#region Methods
	protected sealed override Vector3 CalculateForce ()
	{
        /*
         * You should never override this method nor change its behavior unless
         * youre 100% sure what you're doing.   See SteerForNeighborGroup.
         * 
         * Raise an exception if called. Everything will be calculated by
         * by SteerForNeighborGroup.
         */
		throw new System.NotImplementedException("SteerForNeighbors.CalculateForce should never be called directly.  " +
			"Did you enable a SteerForNeighbors subclass manually? They are disabled by SteerForNeighborGroup on Start.");
	}

	public abstract Vector3 CalculateNeighborContribution(Vehicle other);

	/// <summary>
	/// Initialize this instance.
	/// </summary>
	/// <remarks>Used since SteerForNeighborGroup disables the behaviors, so
	/// Unity may end up not calling their Awake and Start methods.</remarks>
	public void Initialize()
	{
		Awake();
		Start();
	}
	#endregion	
}

}
