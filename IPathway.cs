using UnityEngine;

namespace UnitySteer
{
	public interface IPathway
	{
		bool IsCyclic { get; }
		float TotalPathLength { get; }
		Vector3 FirstPoint { get; }
		Vector3 LastPoint { get; }
		int SegmentCount { get; }

		// Given an arbitrary point ("A"), returns the nearest point ("P") on
		// this path.  Also returns, via output arguments, the path tangent at
		// P and a measure of how far A is outside the Pathway's "tube".  Note
		// that a negative distance indicates A is inside the Pathway.
		Vector3 MapPointToPath (Vector3 point, ref PathRelativePosition tStruct);

		// given a distance along the path, convert it to a point on the path
		Vector3 MapPathDistanceToPoint (float pathDistance);

		// Given an arbitrary point, convert it to a distance along the path.
		float MapPointToPathDistance (Vector3 point);

		// is the given point inside the path tube?
		bool IsInsidePath (Vector3 point);

		// how far outside path tube is the given point?  (negative is inside)
		float HowFarOutsidePath (Vector3 point);

		void DrawGizmos ();
	}
}
