// ----------------------------------------------------------------------------
//
// Written by Ricardo J. Mendez http://www.arges-systems.com/ based on 
// OpenSteer's PolylinePathway.
//
//
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.  IN NO EVENT SHALL
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.
//
//
// ----------------------------------------------------------------------------
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace UnitySteer
{
	/// <summary>
	/// Represents a Pathway created from a list of Vector3s
	/// </summary>
    public class Vector3Pathway : Pathway
    {
        private IList<Vector3>  _path;
        private float       _totalPathLength;
        
        private IList<float>   _lengths;
        private IList<Vector3> _normals;
        private IList<Vector3> _points;
        
		/// <summary>
		/// Minimum radius along the path.
		/// </summary>
        private float       _radius = 0.5f;
		
		
		public IList<Vector3> Path {
			get {
				return _path;
			}
		}
		
        
        public Vector3Pathway () {
			_points = new List<Vector3>(10);
			_lengths = new List<float>(10);
			_normals = new List<Vector3>(10);
        }

		
		/// <summary>
		/// Constructs a Vector3Pathway given an array of points and a path radius
		/// </summary>
		/// <param name="path">
		/// List of Vector3 to be used for the path in world space <see cref="Vector3"/>
		/// </param>
		/// <param name="radius">
		/// Radius to use for the connections <see cref="System.Single"/>
		/// </param>
		/// <param name="cyclic">
		/// Is the path cyclic? <see cref="System.Boolean"/>
		/// </param>
		/// <remarks>The current implementation assumes that all pathways will 
		/// have the same radius.
		/// </remarks>
        public Vector3Pathway (IList<Vector3> path, float radius, bool cyclic)
        {
            Initialize(path, radius, cyclic);
        }

        protected override Vector3 GetFirstPoint()
        {
            return _points.FirstOrDefault();
        }
        
        protected override Vector3 GetLastPoint()
        {
            return _points.LastOrDefault();
        }
        
        protected override float GetTotalPathLength()
        {
            return _totalPathLength;
        }
		
		protected override int GetSegmentCount()
		{
			return _points.Count;
		}
        
		/// <summary>
		/// Constructs the Pathway from a list of Vector3
		/// </summary>
		/// <param name="path">
		/// A list of Vector3 defining the path points in world space<see cref="Vector3"/>
		/// </param>
		/// <param name="radius">
		/// Radius to use for the connections<see cref="System.Single"/>
		/// </param>
		/// <param name="cyclic">
		/// Is the path cyclic?
		/// </param>
        public void Initialize (IList<Vector3> path, float radius, bool cyclic)
        {
            this._path = path;
            this._radius  = radius;
			// TODO: Disregard cyclic, acquire quick test
            this.IsCyclic = false;
            
            // set data members, allocate arrays
            int pointCount = path.Count;
            _totalPathLength = 0;
            if (cyclic) 
            {
                pointCount++;
            }
            _points  = new List<Vector3>(pointCount);
            _lengths = new List<float>(pointCount);
            _normals = new List<Vector3>(pointCount);

            // loop over all points
            for (int i = 0; i < pointCount; i++)
            {
				AddPoint(path[i]);
            }
        }
		
		public void AddPoint(Vector3 point)
		{
		    if (_points.Count > 0)
            {
                // compute the segment length
				var normal = point - _points.Last();
				var length = normal.magnitude;
				_lengths.Add(length);
				_normals.Add(normal / length);
				// keep running total of segment lengths
                _totalPathLength += length;
            }
			else
			{
				_normals.Add(Vector3.zero);
				_lengths.Add(0);
			}
			_points.Add(point);
		}
        
        public override Vector3 MapPointToPath(Vector3 point, ref PathRelativePosition pathRelative)
        {
            float d;
            float minDistance = float.MaxValue;
            Vector3 onPath = Vector3.zero;
			
			pathRelative.segmentIndex = -1;
            // loop over all segments, find the one nearest to the given point
            for (int i = 1; i < _points.Count; i++)
            {
                float   segmentLength = _lengths[i];
                Vector3 segmentNormal = _normals[i];
                Vector3 chosenPoint = Vector3.zero;
                d = OpenSteerUtility.PointToSegmentDistance(point, _points[i-1], _points[i], 
                                                            segmentNormal, segmentLength,  
                                                            ref chosenPoint);
                if (d < minDistance)
                {
					minDistance = d;
					onPath = chosenPoint; 
					pathRelative.tangent = segmentNormal;
					pathRelative.segmentIndex = i;
                }
            }

            // measure how far original point is outside the Pathway's "tube"
            pathRelative.outside = (onPath - point).magnitude - _radius;
			
			// return point on path
            return onPath;
        }

        public override float MapPointToPathDistance(Vector3 point)
        {
			if (_points.Count < 2)
				return 0;

			
            float d;
            float minDistance = float.MaxValue;
            float segmentLengthTotal = 0;
            float pathDistance = 0;

            for (int i = 1; i < _points.Count; i++)
            {
                float   segmentProjection = 0;
                float   segmentLength = _lengths[i];
                Vector3 segmentNormal = _normals[i];
                d = OpenSteerUtility.PointToSegmentDistance(point, _points[i-1], _points[i], 
                                                            segmentNormal, segmentLength, 
                                                            ref segmentProjection);
                if (d < minDistance)
                {
                    minDistance = d;
                    pathDistance = segmentLengthTotal + segmentProjection;
                }
                segmentLengthTotal += segmentLength;
            }

            // return distance along path of onPath point
            return pathDistance;
        }

        public override Vector3 MapPathDistanceToPoint(float pathDistance)
        {
            // clip or wrap given path distance according to cyclic flag
            float remaining = pathDistance;
            if (IsCyclic)
            {
                remaining = pathDistance - (_totalPathLength * Mathf.Floor(pathDistance / _totalPathLength));
            }
            else
            {
                if (pathDistance < 0) 
					return _points.First();
                if (pathDistance >= _totalPathLength) 
					return _points.Last();
            }

            // step through segments, subtracting off segment lengths until
            // locating the segment that contains the original pathDistance.
            // Interpolate along that segment to find 3d point value to return.
            Vector3 result = Vector3.zero;
            for (int i = 1; i < _points.Count; i++)
            {
                float segmentLength = _lengths[i];
                if (segmentLength < remaining)
                {
                    remaining -= segmentLength;
                }
                else
                {
                    float ratio = remaining / segmentLength;
                    result = Vector3.Lerp(_points[i - 1], _points[i], ratio);
                    break;
                }
            }
            return result;
        }
		
		public override void DrawGizmos ()
		{
			for (var i = 0; i < _points.Count - 1; i++)
			{
				Debug.DrawLine(_points[i], _points[i+1], Color.green);
			}
		}
    }
}