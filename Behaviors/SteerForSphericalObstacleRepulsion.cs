// #define ANNOTATE_AVOIDOBSTACLES
using UnityEngine;
using UnitySteer;
using UnitySteer.Helpers;
using System.Linq;

/// <summary>
/// Steers a vehicle to be repulsed by stationary obstacles
/// </summary>
/// <remarks>
/// For every obstacle detected, this will:
/// 1) Add up a repulsion vector that is the distance between the vehicle 
/// and the obstacle, divided by the squared magnitude of the distance 
/// between the obstacle and the vehicle's future intended position. This is
/// done because the further an obstacle is from our desired position, the
/// least we care about it (which could have side effects when dealing with
/// very large obstacles which we don't happen to intersect, need to review).
/// 2) If we would intersect this obstacle on our current path, then we 
/// multiply this repulsion vector by a factor of the number of detected
/// (since the others might just be to the side and we want to give it
/// higher weight).
/// 3) Divide the total by the number of obstacles.
/// The final correction vector is the old desired velocity reflected 
/// along the calculated avoidance vector.
/// </remarks>
[AddComponentMenu("UnitySteer/Steer/... for SphericalObstacleRepulsion")]
public class SteerForSphericalObstacleRepulsion : Steering
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
	float _estimationTime = 2;
	#endregion


	public override bool IsPostProcess 
	{ 
		get { return true; }
	}

	
	#region Public properties
	/// <summary>
	/// How far in the future to estimate the vehicle position
	/// </summary>
	public float EstimationTime {
		get {
			return this._estimationTime;
		}
		set {
			_estimationTime = value;
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
	/// are within MinTimeToCollision, and calculate a repulsion vector based
	/// on them.
	/// </remarks>
	protected override Vector3 CalculateForce()
	{
		Vector3 avoidance = Vector3.zero;
		if (Vehicle.Radar.Obstacles == null || !Vehicle.Radar.Obstacles.Any())
		{
			return avoidance;
		}

		/*
		 * While we could just calculate movement as (Velocity * predictionTime) 
		 * and save ourselves the substraction, this allows other vehicles to
		 * override PredictFuturePosition for their own ends.
		 */
		Vector3 futurePosition = Vehicle.PredictFutureDesiredPosition(_estimationTime);
		
		#if ANNOTATE_AVOIDOBSTACLES
		Debug.DrawLine(Vehicle.Position, futurePosition, Color.cyan);
		#endif
		
		// test all obstacles for intersection with my forward axis,
		// select the one whose point of intersection is nearest
		Profiler.BeginSample("Accumulate spherical obstacle influences");
		for (int i = 0; i < Vehicle.Radar.Obstacles.Count; i++)
		{
			var sphere = Vehicle.Radar.Obstacles[i];
			if (sphere == null || sphere.Equals(null)) continue; // In case the object was destroyed since we cached it
			PathIntersection next = FindNextIntersectionWithSphere(Vehicle.Position, futurePosition, sphere);
			float avoidanceMultiplier = 1;
			if (next.Intersect) {
				#if ANNOTATE_AVOIDOBSTACLES
				Debug.DrawRay(Vehicle.Position, Vehicle.DesiredVelocity.normalized * next.Distance, Color.yellow);
				#endif
				avoidanceMultiplier = Vehicle.Radar.Obstacles.Count;
			}

			var distanceCurrent = Vehicle.Position - sphere.Position;
			var distanceFuture = futurePosition - sphere.Position;
			avoidance += avoidanceMultiplier * distanceCurrent / distanceFuture.sqrMagnitude;
		}
		Profiler.EndSample();

		avoidance /= Vehicle.Radar.Obstacles.Count;


		var newDesired = Vector3.Reflect(Vehicle.DesiredVelocity, avoidance);

		#if ANNOTATE_AVOIDOBSTACLES
		Debug.DrawLine(Vehicle.Position, Vehicle.Position + avoidance, Color.green);
		Debug.DrawLine(Vehicle.Position, futurePosition, Color.blue);
		Debug.DrawLine(Vehicle.Position, Vehicle.Position + newDesired, Color.white);
		#endif

		return newDesired;
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