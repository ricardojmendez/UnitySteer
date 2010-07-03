using UnityEngine;

public class RandomizePosition: MonoBehaviour
{
	public float Radius = 3;
	public bool IsPlanar = false;
	
	void Start()
	{
		var pos = Random.insideUnitSphere * Radius;
		var rot = Random.insideUnitSphere;
		
		if (IsPlanar)
		{
			pos.y = 0;
			rot.x = 0;
			rot.z = 0;
		}
		
		transform.position += pos * Radius;
		transform.rotation = Quaternion.Euler(rot * 360);
		Destroy(this);
	}
}