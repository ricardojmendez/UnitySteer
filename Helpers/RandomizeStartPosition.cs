using UnityEngine;

public class RandomizeStartPosition: MonoBehaviour
{
	public Vector3 Radius = Vector3.one;

	public bool IsPlanar = false;
	public bool RandomizeRotation = true;
	
	void Start()
	{
		var pos = Vector3.Scale(Random.insideUnitSphere, Radius);
		var rot = Random.insideUnitSphere;
		
		if (IsPlanar)
		{
			pos.y = 0;
			rot.x = 0;
			rot.z = 0;
		}
		
		transform.position += pos;

		if (RandomizeRotation) {
			transform.rotation = Quaternion.Euler(rot * 360);	
		}
		Destroy(this);
	}
}