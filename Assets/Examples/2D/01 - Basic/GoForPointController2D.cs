using System.Collections;

using UnityEngine;

using UnitySteer2D;
using UnitySteer2D.Behaviors;

[RequireComponent(typeof(AutonomousVehicle2D)), RequireComponent(typeof(SteerForPoint2D))]
public class GoForPointController2D : MonoBehaviour 
{
    SteerForPoint2D _steering;

	[SerializeField]
    Vector2 _pointRange = Vector2.one * 5f;

	void Start() 
	{
        _steering = GetComponent<SteerForPoint2D>();
		_steering.OnArrival += (_) => FindNewTarget();
		FindNewTarget();
	}


	void FindNewTarget()
	{
		_steering.TargetPoint = Vector2.Scale(Random.onUnitSphere, _pointRange);
		_steering.enabled = true;
	}
}
