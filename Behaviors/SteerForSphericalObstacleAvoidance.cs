//#define ANNOTATE_AVOIDOBSTACLES
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
		 * While we could just calculate movement as (Velocity * predictionTime) 
		 * and save ourselves the substraction, this allows other vehicles to
		 * override PredictFuturePosition for their own ends.
		 */
		Vector3 futurePosition = Vehicle.PredictFutureDesiredPosition(_minTimeToCollision);
		Vector3 movement = futurePosition - Vehicle.Position;
		
		#if ANNOTATE_AVOIDOBSTACLES
		Debug.DrawLine(Vehicle.Position, futurePosition, Color.cyan);
		#endif
		
		// test all obstacles for intersection with my forward axis,
		// select the one whose point of intersection is nearest
		Profiler.BeginSample("Find nearest intersection");
		foreach (var o in Vehicle.Radar.Obstacles)
		{
			var sphere = o as DetectableObject;
			PathIntersection next = FindNextIntersectionWithSphere(Vehicle.Position, futurePosition, sphere);
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
			nearest.Distance < movement.magnitude)
		{
			#if ANNOTATE_AVOIDOBSTACLES
			Debug.DrawLine(Vehicle.Position, nearest.Obstacle.Position, Color.red);
			#endif

			// compute avoidance steering force: take offset from obstacle to me,
			// take the component of that which is lateral (perpendicular to my
			// movement direction),  add a bit of forward component
			Vector3 offset = Vehicle.Position - nearest.Obstacle.Position;
			Vector3 moveDirection = movement.normalized;
			avoidance =	 OpenSteerUtility.perpendicularComponent(offset, moveDirection);

			avoidance.Normalize();

			#if ANNOTATE_AVOIDOBSTACLES
			Debug.DrawLine(Vehicle.Position, Vehicle.Position + avoidance, Color.white);
			#endif

			avoidance += moveDirection * Vehicle.MaxForce * _avoidanceForceFactor;

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
	/// <param name="vehiclePosition">
	/// The current position of the vehicle
	/// </param>
	/// <param name="futureVehiclePosition">
	/// The position where we expect the vehicle to be soon
	/// </param>
	/// <param name="obstacle">
	/// A spherical obstacle to check against <see cref="DetectableObject"/>
	/// </param>
	/// <returns>
	/// A PathIntersection with the intersection details <see cref="PathIntersection"/>
	/// </returns>
	public PathIntersection FindNextIntersectionWithSphere(Vector3 vehiclePosition, Vector3 futureVehiclePosition, DetectableObject obstacle) {
		// this mainly follows http://www.lighthouse3d.com/tutorials/maths/ray-sphere-intersection/
		
		var intersection = new PathIntersection(obstacle);
		
		float combinedRadius = Vehicle.ScaledRadius + obstacle.ScaledRadius;
		var movement = futureVehiclePosition - vehiclePosition;
		var direction = movement.normalized;
		
		var vehicleToObstacle = obstacle.Position - vehiclePosition;
		
		// this is the length of vehicleToObstacle projected onto direction
		float projectionLength = Vector3.Dot(direction, vehicleToObstacle);
		
		// if the projected obstacle center lies further away than our movement + both radius, we're not going to collide
		if (projectionLength > movement.magnitude + combinedRadius) {
			//print("no collision - 1");
			return intersection;
		}
		
		// the foot of the perpendicular
		var projectedObstacleCenter = vehiclePosition + projectionLength * direction;
		
		// distance of the obstacle to the pathe the vehicle is going to take
		float obstacleDistanceToPath = (obstacle.Position - projectedObstacleCenter).magnitude;
		//print("obstacleDistanceToPath: " + obstacleDistanceToPath);
		
		// if the obstacle is further away from the movement, than both radius, there's no collision
		if (obstacleDistanceToPath > combinedRadius) {
			//print("no collision - 2");
			return intersection;
		}

		// use pythagorean theorem to calculate distance out of the sphere (if you do it 2D, the line through the circle would be a chord and we need half of its length)
		float halfChord = Mathf.Sqrt(combinedRadius * combinedRadius + obstacleDistanceToPath * obstacleDistanceToPath);
		
		// if the projected obstacle center lies opposite to the movement direction (aka "behind")
		if (projectionLength < 0) {
			// behind and further away than both radius -> no collision (we already passed)
			if (vehicleToObstacle.magnitude > combinedRadius)
				return intersection;
			
			var intersectionPoint = projectedObstacleCenter - direction * halfChord;
			intersection.Intersect = true;
			intersection.Distance = (intersectionPoint - vehiclePosition).magnitude;
			return intersection;
		}
		
		// calculate both intersection points
		var intersectionPoint1 = projectedObstacleCenter - direction * halfChord;
		var intersectionPoint2 = projectedObstacleCenter + direction * halfChord;

		// pick the closest one
		float intersectionPoint1Distance = (intersectionPoint1 - vehiclePosition).magnitude;
		float intersectionPoint2Distance = (intersectionPoint2 - vehiclePosition).magnitude;
		
		intersection.Intersect = true;
		intersection.Distance = Mathf.Min(intersectionPoint1Distance, intersectionPoint2Distance);
		
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