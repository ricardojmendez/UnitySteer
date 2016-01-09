using UnityEngine;
using UnitySteer.Attributes;

namespace UnitySteer2D.Tools
{
    /// <summary>
    /// Randomizes a transform's position (and possibly rotation) on start, then self-destructs.
    /// </summary>
    [AddComponentMenu("UnitySteer2D/Tools/Randomize Start Position ")]
    public class RandomizeStartPosition2D : MonoBehaviour
	{
        [SerializeField]
        [Behaviour2D]
        protected string _2DTool = "This tool is to be used with 2D Behaviours and Objects only.";

        public Vector2 Radius = Vector2.one;

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
			var pos = Vector2.Scale(Random.insideUnitSphere, Radius);
            pos = Vector2.Scale(pos, AllowedAxes);
            transform.position += (Vector3)pos;

			if (RandomizeRotation) 
			{
				var rot = Random.insideUnitSphere;

                AllowedAxes.x = 0;
                AllowedAxes.y = 0;

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