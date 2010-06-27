using UnityEngine;
using System.Collections;

namespace UnitySteer.Helpers
{
	[System.Serializable]
	public class Tick {
		/// <summary>
		/// Last time the object was ticked
		/// </summary>
		float nextTick = 0;
		
		/// <summary>
		/// How many ms should pass before the object is ticked again
		/// </summary>
		public float TickLapse = 0.1f;
		
		/// <summary>
		/// Tick priority (higher goes first)
		/// </summary>
		public float Priority = 0;
		

		/// <summary>
		/// Will return true if we need to tick the current behavior
		/// </summary>
		public bool ShouldTick 
		{
			get
			{
				var time = Time.time;
				var result = nextTick < time;
				if (result)
				{
					nextTick = time + TickLapse;
				}
				return result;
			}
		}
		
	}
}