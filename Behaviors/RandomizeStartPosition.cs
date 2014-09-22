using UnityEngine;
using UnitySteer.Attributes;

namespace UnitySteer.Behaviors
{

	/// <summary>
	/// Randomizes a transform's position (and possibly rotation) on start, then self-destructs.
	/// </summary>
	public class RandomizeStartPosition: MonoBehaviour
	{
		public Vector3 Radius = Vector3.one;


		public bool RandomizeRotation = true;

		/// <summary>
		/// Allowed axes for translation/rotation
		/// </summary>
		/// <remarks>>While it would appear to be redundant with the radius - we could just zero
		/// one of the radius values if we dont want to change its position along that axis - that
		/// does not solve the issue of the rotation, where we may also want to limit the
		/// axes changed. </remarks>
		[Vector3Toggle] public Vector3 AllowedAxes = Vector3.one;

		void Start()
		{
			var pos = Vector3.Scale(Random.insideUnitSphere, Radius);
			pos = Vector3.Scale(pos, AllowedAxes);
			transform.position += pos;

			if (RandomizeRotation) 
			{
				var rot = Random.insideUnitSphere;


				if (AllowedAxes.y == 0)
				{
					rot.x = 0;
					rot.z = 0;
				}
				if (AllowedAxes.x == 0)
				{
					rot.y = 0;
					rot.z = 0;
				}
				if (AllowedAxes.z == 0)
				{
					rot.x = 0;
					rot.y = 0;
				}

				transform.rotation = Quaternion.Euler(rot * 360);	
			}
			Destroy(this);
		}
	}

}