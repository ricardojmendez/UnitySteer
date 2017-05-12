using UnityEngine;
using System.Collections;
using UnitySteer2D.Behaviors;

public class DestroyOnCollision2D : MonoBehaviour 
{

	void OnTriggerEnter2D(Collider2D other)
	{
		if (Time.time < 2) return;

		var vehicle = other.GetComponent<Vehicle2D>();
		if (vehicle)
		{
			var renderer = other.GetComponentInChildren<Renderer>();
			renderer.material.color = Color.blue;

			Debug.LogWarning(string.Format("{2} Destroying {0} upon collision with {1}", gameObject, vehicle, Time.time));
			Destroy(gameObject);
		}
	}

}
