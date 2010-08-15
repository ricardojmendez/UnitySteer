#define ANNOTATE_AVOIDNEIGHBORS
using UnityEngine;

public class SteerForNeighborAvoidance : Steering {
	
	#region Private fields
	[SerializeField]
	float _avoidAngleCos = 0.707f;
	
	[SerializeField]
	float _minTimeToCollision = 2f;
	#endregion
	
	
	
	#region Public properties
	/// <summary>
	/// Cosine of the angle limit for the approach that a neighbor must be 
	/// coming at before we avoid it
	/// </summary>
	/// <value>
	/// The avoid angle cos.
	/// </value>
	public float AvoidAngleCos {
		get {
			return this._avoidAngleCos;
		}
		set {
			_avoidAngleCos = value;
		}
	}

	
	/// <summary>
	/// Minimum time to collision
	/// </summary>
	public float MinTimeToCollision {
		get {
			return this._minTimeToCollision;
		}
		set {
			_minTimeToCollision = value;
		}
	}
	#endregion
	
	

	/// <summary>
	/// Should the force be calculated?
	/// </summary>
	/// <returns>
	/// A <see cref="Vector3"/>
	/// </returns>
	protected override Vector3 CalculateForce()
	{
		/*
		// first priority is to prevent immediate interpenetration
		Vector3 separation = steerToAvoidCloseNeighbors (0, others);
		if (separation != Vector3.zero) 
		{
			return separation;
		}
		*/

		// otherwise, go on to consider potential future collisions
		float steer = 0;
		Vehicle threat = null;

		// Time (in seconds) until the most immediate collision threat found
		// so far.	Initial value is a threshold: don't look more than this
		// many frames into the future.
		float minTime = _minTimeToCollision;

		Vector3 threatPositionAtNearestApproach = Vector3.zero;
		#if ANNOTATE_AVOIDNEIGHBORS
		Vector3 ourPositionAtNearestApproach = Vector3.zero;
		#endif

		// for each of the other vehicles, determine which (if any)
		// pose the most immediate threat of collision.
		foreach (var other in Vehicle.Radar.Vehicles)
		{
			if (other != this)
			{	
				// avoid when future positions are this close (or less)
				float collisionDangerThreshold = Vehicle.ScaledRadius + other.ScaledRadius;
				
				// predicted time until nearest approach of "this" and "other"
				float time = Vehicle.PredictNearestApproachTime (other);
				
				// If the time is in the future, sooner than any other
				// threatened collision...
				if ((time >= 0) && (time < minTime))
				{
					// if the two will be close enough to collide,
					// make a note of it
					Vector3 ourPos = Vector3.zero;
					Vector3 hisPos = Vector3.zero;
					float	dist   = Vehicle.ComputeNearestApproachPositions (other, time, ref ourPos, ref hisPos);
					
					if (dist < collisionDangerThreshold)
					{
						minTime = time;
						threat = other;
						threatPositionAtNearestApproach = hisPos;
						#if ANNOTATE_AVOIDNEIGHBORS
						ourPositionAtNearestApproach = ourPos;
						#endif
					}
				}
			}
		}

		// if a potential collision was found, compute steering to avoid
		if (threat != null)
		{
			// parallel: +1, perpendicular: 0, anti-parallel: -1
			float parallelness = Vector3.Dot(transform.forward, threat.transform.forward);
			// Debug.Log("Parallel "+parallelness + " "+avoidAngleCos+" "+threatPositionAtNearestApproach);

			if (parallelness < -_avoidAngleCos)
			{
				// anti-parallel "head on" paths:
				// steer away from future threat position
				Vector3 offset = threatPositionAtNearestApproach - Vehicle.Position;
				float sideDot = Vector3.Dot(offset, transform.right);
				steer = (sideDot > 0) ? -1.0f : 1.0f;
			}
			else if (parallelness > _avoidAngleCos)
			{
				// parallel paths: steer away from threat
				Vector3 offset = threat.Position - Vehicle.Position;
				float sideDot = Vector3.Dot(offset, transform.right);
				steer = (sideDot > 0) ? -1.0f : 1.0f;
			}
			else 
			{
				/* 
					Perpendicular paths: steer behind threat

					Only the slower vehicle attempts this, unless that 
					slower vehicle is static.  If both have the same
					speed, then the one with the lowest index falls
					behind.					
					
					Something to test is making a slower vehicle fall
					behind, while a faster vehicle cuts ahead.
				 */
				if (Vehicle.Speed < threat.Speed
						 || threat.Speed == 0
				    	 || gameObject.GetInstanceID() < threat.gameObject.GetInstanceID()) 
				{
					float sideDot = Vector3.Dot(transform.right, threat.Velocity);
					steer = (sideDot > 0) ? -1.0f : 1.0f;
				}
			}
			
			/* Steer will end up being applied as a multiplier to the
			   vehicle's side vector. If we simply apply te -1/+1 being
			   assigned above, then we'll end up with a unit displacement
			   from the other object's position. We should account for
			   both its radius and our own.
			 */
			steer *= Vehicle.ScaledRadius + threat.ScaledRadius;

			#if ANNOTATE_AVOIDNEIGHBORS
			AnnotateAvoidNeighbor (threat,
								   steer,
								   ourPositionAtNearestApproach,
								   threatPositionAtNearestApproach);
			#endif
		}

		return transform.right * steer;
	}
	
	#if ANNOTATE_AVOIDNEIGHBORS
	protected virtual void AnnotateAvoidNeighbor (Vehicle vehicle, float steer, Vector3 position, Vector3 threatPosition)
	{
		Debug.DrawLine(Vehicle.Position, vehicle.Position, Color.red); // Neighbor position
		Debug.DrawLine(Vehicle.Position, position, Color.green);	   // Position we're aiming for
		Debug.DrawLine(Vehicle.Position, threatPosition, Color.magenta);	// Calculated threat position
	}
	#endif
	
}