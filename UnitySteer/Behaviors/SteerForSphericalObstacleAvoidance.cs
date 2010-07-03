#define ANNOTATE_AVOIDOBSTACLES
using System.Collections.Generic;
using UnityEngine;
using UnitySteer;
using UnitySteer.Helpers;

/// <summary>
/// Steers a vehicle to avoid stationary obstacles
/// </summary>
public class SteerForSphericalObstacleAvoidance : Steering
{
	#region Structs
	public struct PathIntersection
	{
		public bool intersect;
		public float distance;

		// The two below are not used??

		//public Vector3 surfacePoint;
		//public Vector3 surfaceNormal;
		public SphericalObstacle obstacle;
		
		public PathIntersection(SphericalObstacle obstacle)
		{
			this.obstacle = obstacle;
			intersect = false;
			distance = float.MaxValue;
		}
	};	
	#endregion
	
	#region Private fields
	[SerializeField]
	float _avoidanceForceFactor = 0.75f;

	[SerializeField]
	float _minTimeToCollision = 2;
	#endregion
	
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
	
	protected new void Start()
	{
		base.Start();
		Vehicle.Radar.ObstacleFactory = new ObstacleFactory(SphericalObstacle.GetObstacle);
	}
	
	
	protected override Vector3 CalculateForce()
	{
		/*
		Debug.Log("Steering from "  + Vehicle.Radar.Obstacles.Count);
		foreach (var o in Vehicle.Radar.Obstacles)
		{
			if (o != null)
			{
				Debug.Log(o.ToString());
			}
		}
		Debug.Log("-----------------");
		*/
		
		Vector3 avoidance = Vector3.zero;
		if (Vehicle.Radar.Obstacles == null || Vehicle.Radar.Obstacles.Count == 0)
		{
			return avoidance;
		}

		PathIntersection nearest = new PathIntersection(null);
		float minDistanceToCollision = _minTimeToCollision * Vehicle.Speed;

		// test all obstacles for intersection with my forward axis,
		// select the one whose point of intersection is nearest
		foreach (var o in Vehicle.Radar.Obstacles)
		{
			SphericalObstacle sphere = o as SphericalObstacle;
			// xxx this should be a generic call on Obstacle, rather than
			// xxx this code which presumes the obstacle is spherical
			PathIntersection next = FindNextIntersectionWithSphere (sphere);
			if (!nearest.intersect ||
				(next.intersect &&
				 next.distance < nearest.distance))
			{
				nearest = next;
			}
		}


		// when a nearest intersection was found
		if (nearest.intersect &&
			nearest.distance < minDistanceToCollision)
		{
			#if ANNOTATE_AVOIDOBSTACLES
			Debug.DrawLine(transform.position, nearest.obstacle.center, Color.red);
			#endif

			// compute avoidance steering force: take offset from obstacle to me,
			// take the component of that which is lateral (perpendicular to my
			// forward direction), set length to maxForce, add a bit of forward
			// component (in capture the flag, we never want to slow down)
			Vector3 offset = transform.position - nearest.obstacle.center;
			avoidance =	 OpenSteerUtility.perpendicularComponent(offset, transform.forward);

			avoidance.Normalize();
			avoidance *= Vehicle.MaxForce;
			avoidance += transform.forward * Vehicle.MaxForce * _avoidanceForceFactor;
		}

		return avoidance;
	}
	
	
	/// <summary>
	/// Finds the vehicle's next intersection with a spherical obstacle
	/// </summary>
	/// <param name="obs">
	/// A spherical obstacle to check against <see cref="SphericalObstacle"/>
	/// </param>
	/// <returns>
	/// A PathIntersection with the intersection details <see cref="PathIntersection"/>
	/// </returns>
	public PathIntersection FindNextIntersectionWithSphere (SphericalObstacle obs)
	{
		// This routine is based on the Paul Bourke's derivation in:
		//	 Intersection of a Line and a Sphere (or circle)
		//	 http://www.swin.edu.au/astronomy/pbourke/geometry/sphereline/

		float b, c, d, p, q, s;
		Vector3 lc;

		// initialize pathIntersection object
		PathIntersection intersection = new PathIntersection(obs);
		// find "local center" (lc) of sphere in the vehicle's coordinate space
		lc = transform.InverseTransformPoint(obs.center);
		
		#if ANNOTATE_AVOIDOBSTACLES
		obs.annotatePosition();
		#endif
		
		// computer line-sphere intersection parameters
		b = -2 * lc.z;
		c = Mathf.Pow(lc.x, 2) + Mathf.Pow(lc.y, 2) + Mathf.Pow(lc.z, 2) - 
			Mathf.Pow(obs.radius + Vehicle.Radius, 2);
		d = (b * b) - (4 * c);

		// when the path does not intersect the sphere
		if (d < 0) return intersection;

		// otherwise, the path intersects the sphere in two points with
		// parametric coordinates of "p" and "q".
		// (If "d" is zero the two points are coincident, the path is tangent)
		s = (float) System.Math.Sqrt(d);
		p = (-b + s) / 2;
		q = (-b - s) / 2;

		// both intersections are behind us, so no potential collisions
		if ((p < 0) && (q < 0)) return intersection; 

		// at least one intersection is in front of us
		intersection.intersect = true;
		intersection.distance =
			((p > 0) && (q > 0)) ?
			// both intersections are in front of us, find nearest one
			((p < q) ? p : q) :
			// otherwise only one intersections is in front, select it
			((p > 0) ? p : q);
		
		return intersection;
	}
	
}