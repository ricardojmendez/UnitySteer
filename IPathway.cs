using UnityEngine;

namespace UnitySteer
{
    public struct PathRelativePosition
    {
        public float Outside;
        public Vector3 Tangent;
        public int SegmentIndex;
    }

    public interface IPathway
    {
        float TotalPathLength { get; }
        Vector3 FirstPoint { get; }
        Vector3 LastPoint { get; }
        int SegmentCount { get; }
        float Radius { get; set; }

        /// <summary>
        /// Given an arbitrary point ("A"), returns the nearest point ("P") on
        /// this path.  Also returns, via output arguments, the path Tangent at
        /// P and a measure of how far A is Outside the Pathway's "tube".  Note
        /// that a negative distance indicates A is inside the Pathway.
        /// </summary>
        /// <param name="point">Reference point.</param>
        /// <param name="pathRelative">Structure indicating the relative path position.</param>
        /// <returns>The closest point to the received reference point.</returns>
        Vector3 MapPointToPath(Vector3 point, ref PathRelativePosition pathRelative);

        /// <summary>
        /// Given a distance along the path, convert it to a specific point on the path.
        /// </summary>
        /// <param name="pathDistance">Path distance to calculate corresponding point for.</param>
        /// <returns>The corresponding path point to the path distance.</returns>
        Vector3 MapPathDistanceToPoint(float pathDistance);

        /// <summary>
        /// Maps the reference point to a distance along the path.
        /// </summary>
        /// <param name="point">Reference point.</param>
        /// <returns>The distance along the path for the point.</returns>
        float MapPointToPathDistance(Vector3 point);

        /// <summary>
        /// Determines whether the received point is inside the path.
        /// </summary>
        /// <param name="point">Point to evaluate.</param>
        /// <returns><c>true</c> if the point is inside the path; otherwise, <c>false</c>.</returns>
        bool IsInsidePath(Vector3 point);

        /// <summary>
        /// Calculates how far Outside the path is the reference point.
        /// </summary>
        /// <param name="point">Reference point.</param>
        /// <returns>How far Outside the path is the reference point.</returns>
        float HowFarOutsidePath(Vector3 point);

        void DrawGizmos();
    }
}