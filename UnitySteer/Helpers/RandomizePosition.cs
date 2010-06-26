using UnityEngine;

public class RandomizePosition: MonoBehaviour
{
	public float Radius = 3;
	
	void Start()
	{
		transform.position += Random.insideUnitSphere * Radius;
		transform.rotation = Quaternion.Euler(Random.insideUnitSphere * 360);
		Destroy(this);
	}
}