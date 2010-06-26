using System;
using UnityEngine;

namespace UnitySteer.Helpers
{
	public class VehicleHelper
	{
		/// <summary>
		/// Calculates if a vehicle is in the neighborhood of another
		/// </summary>
		/// <param name="first">
		/// Vehicle checking <see cref="Vehicle"/>
		/// </param>
		/// <param name="other">
		/// Another vehicle to check against<see cref="Vehicle"/>
		/// </param>
		/// <param name="minDistance">
		/// Minimum distance <see cref="System.Single"/>
		/// </param>
		/// <param name="maxDistance">
		/// Maximum distance <see cref="System.Single"/>
		/// </param>
		/// <param name="cosMaxAngle">
		/// Cosine of the maximum angle between vehicles (for performance)<see cref="System.Single"/>
		/// </param>
		/// <returns>
		/// True if within the neighborhood, or false if otherwise<see cref="System.Boolean"/>
		/// </returns>
		/// <remarks>Originally SteerLibrary.inBoidNeighborhood</remarks>
		public static bool InNeighborhood (Vehicle first, Vehicle other, float minDistance, float maxDistance, float cosMaxAngle)
		{
			if (other == first)
			{
				return false;
			}
			else
			{
				Vector3 offset = other.transform.position - first.transform.position;
				float distanceSquared = offset.sqrMagnitude;

				// definitely in neighborhood if inside minDistance sphere
				if (distanceSquared < (minDistance * minDistance))
				{
					return true;
				}
				else
				{
					// definitely not in neighborhood if outside maxDistance sphere
					if (distanceSquared > (maxDistance * maxDistance))
					{
						return false;
					}
					else
					{
						// otherwise, test angular offset from forward axis
						Vector3 unitOffset = offset / (float) System.Math.Sqrt (distanceSquared);
						float forwardness = Vector3.Dot(first.transform.forward, unitOffset);
						return forwardness > cosMaxAngle;
					}
				}
			}
		}
	}
}

