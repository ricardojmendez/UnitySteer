#define ANNOTATE_AVOIDOBSTACLES
using UnityEngine;
using UnitySteer;
using UnitySteer.Helpers;
using System.Linq;

/// <summary>
/// Steers a vehicle to avoid stationary obstacles
/// </summary>
[AddComponentMenu("UnitySteer/Steer/... for SphericalObstacleAvoidance")]
public class SteerForSphericalObstacleAvoidance : Steering
{
	#region Structs
	public struct PathIntersection
	{
		bool _intersect;
		float _distance;
		DetectableObject _obstacle;
		
		public bool Intersect 
		{ 
			get { return _intersect; }
			set { _intersect = value; }
		}
		
		public float Distance 
		{ 
			get { return _distance; }
			set { _distance = value; }
		}
		

		public DetectableObject Obstacle 
		{ 
			get { return _obstacle; } 
			set { _obstacle = value; }
		}
		
		public PathIntersection(DetectableObject obstacle)
		{
			_obstacle = obstacle;
			_intersect = false;
			_distance = float.MaxValue;
		}
	};	
	#endregion
	
	#region Private fields
	[SerializeField]
	float _avoidanceForceFactor = 0.75f;

	[SerializeField]
	float _minTimeToCollision = 2;
	#endregion


	public override bool IsPostProcess 
	{ 
		get { return true; }
	}

	
	#region Public properties
	/// <summary>
	/// Multiplier for the force applied on avoidance
	/// </summary>
	/// <remarks>If his value is set to 1, the behavior will return an
	/// avoidance force that uses the full brunt of the vehicle's maximum
	/// force.</remarks>
	public float AvoidanceForceFactor {
		get {
			return this._avoidanceForceFactor;
		}
		set {
			_avoidanceForceFactor = value;
		}
	}

	/// <summary>
	/// Minimum time to collision to consider
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
	/// Calculates the force necessary to avoid the closest spherical obstacle
	/// </summary>
	/// <returns>
	/// Force necessary to avoid an obstacle, or Vector3.zero
	/// </returns>
	/// <remarks>
	/// This method will iterate through all detected spherical obstacles that 
	/// are within MinTimeToCollision, and steer to avoid the closest one to the 
	/// vehicle.  It's not ideal, as that means the vehicle might crash into
	/// another obstacle while avoiding the closest one, but it'll do.
	/// </remarks>
	protected override Vector3 CalculateForce()
	{
		Vector3 avoidance = Vector3.zero;
		if (Vehicle.Radar.Obstacles == null || !Vehicle.Radar.Obstacles.Any())
		{
			return avoidance;
		}

		PathIntersection nearest = new PathIntersection(null);
		/*
		 * While we could just calculate line as (Velocity * predictionTime) 
		 * and save ourselves the substraction, this allows other vehicles to
		 * override PredictFuturePosition for their own ends.
		 */
		Vector3 futurePosition = Vehicle.PredictFuturePosition(_minTimeToCollision);
		Vector3 line = (futurePosition - Vehicle.Position);

		// test all obstacles for intersection with my forward axis,
		// select the one whose point of intersection is nearest
		Profiler.BeginSample("Find nearest intersection");
		foreach (var o in Vehicle.Radar.Obstacles)
		{
			var sphere = o as DetectableObject;
			PathIntersection next = FindNextIntersectionWithSphere (sphere, line);
			if (!nearest.Intersect ||
				(next.Intersect &&
				 next.Distance < nearest.Distance))
			{
				nearest = next;
			}
		}
		Profiler.EndSample();


		// when a nearest intersection was found
		Profiler.BeginSample("Calculate avoidance");
		if (nearest.Intersect &&
			nearest.Distance < line.magnitude)
		{
			#if ANNOTATE_AVOIDOBSTACLES
			Debug.DrawLine(Vehicle.Position, nearest.Obstacle.Position, Color.red);
			#endif

			// compute avoidance steering force: take offset from obstacle to me,
			// take the component of that which is lateral (perpendicular to my
			// forward direction),  add a bit of forward component
			Vector3 offset = Vehicle.Position - nearest.Obstacle.Position;
			avoidance =	 OpenSteerUtility.perpendicularComponent(offset, transform.forward);

			avoidance.Normalize();

			#if ANNOTATE_AVOIDOBSTACLES
			Debug.DrawLine(Vehicle.Position, Vehicle.Position + avoidance, Color.white);
			#endif

			avoidance += transform.forward * Vehicle.MaxForce * _avoidanceForceFactor;

			#if ANNOTATE_AVOIDOBSTACLES
			Debug.DrawLine(Vehicle.Position, Vehicle.Position + avoidance, Color.yellow);
			#endif
		}
		Profiler.EndSample();

		return avoidance;
	}
	
	/// <summary>
	/// Finds the vehicle's next intersection with a spherical obstacle
	/// </summary>
	/// <param name="obs">
	/// A spherical obstacle to check against <see cref="DetectableObject"/>
	/// </param>
	/// <param name="line">
	/// Line that we expect we'll follow to our future destination
	/// </param>
	/// <returns>
	/// A PathIntersection with the intersection details <see cref="PathIntersection"/>
	/// </returns>
	public PathIntersection FindNextIntersectionWithSphere (DetectableObject obs, Vector3 line)
	{
		/*
		 * This routine is based on the Paul Bourke's derivation in:
		 *   Intersection of a Line and a Sphere (or circle)
		 *   http://www.swin.edu.au/astronomy/pbourke/geometry/sphereline/
		 *
		 * Retaining the same variable values used in that description.
		 * 
		 */
		float a, b, c, bb4ac;
		var toCenter = Vehicle.Position - obs.Position;

		// initialize pathIntersection object
		var intersection = new PathIntersection(obs);
		
		#if ANNOTATE_AVOIDOBSTACLES
		Debug.DrawLine(Vehicle.Position, Vehicle.Position + line, Color.cyan);
		#endif
		
		// computer line-sphere intersection parameters
		a = line.magnitude;
		b = 2 * Vector3.Dot(line, toCenter);
		c = obs.Position.magnitude;
		c += Vehicle.Position.magnitude;
		c -= 2 * Vector3.Dot(obs.Position, Vehicle.Position); 
		c -= Mathf.Pow(obs.ScaledRadius + Vehicle.ScaledRadius, 2);
		bb4ac = b * b - 4 * a * c;

		if (bb4ac >= 0)  {
			intersection.Intersect = true;
			Vector3 closest = Vector3.zero;
			if (bb4ac == 0) {
				// Only one intersection
				var mu = -b / (2*a);
				closest = mu * line;
			}
			else {
				// More than one intersection
				var mu1 = (-b + Mathf.Sqrt(bb4ac)) / (2*a);
				var mu2 = (-b - Mathf.Sqrt(bb4ac)) / (2*a);
				/*
				 * If the results are negative, the obstacle is behind us.
				 * 
				 * If one result is negative and the other one positive,
				 * that would indicate that one intersection is behind us while
				 * the other one ahead of us, which would mean that we're 
				 * just overlapping the obstacle, so we should still avoid.  
				 */
				if (mu1 < 0 && mu2 < 0)
					intersection.Intersect = false;
				else
					closest = (Mathf.Abs(mu1) < Mathf.Abs (mu2)) ? mu1 * line : mu2 * line;
			}
			#if ANNOTATE_AVOIDOBSTACLES
			Debug.DrawRay(Vehicle.Position, closest, Color.red);
			#endif

			intersection.Distance =  closest.magnitude;
		}
		return intersection;
	}
	
	#if ANNOTATE_AVOIDOBSTACLES
	void OnDrawGizmos()
	{
		if (Vehicle != null)
		{
			foreach (var o in Vehicle.Radar.Obstacles)
			{
				Gizmos.color = Color.red;
				Gizmos.DrawWireSphere(o.Position, o.ScaledRadius);
			}
		}
	}
	#endif
}