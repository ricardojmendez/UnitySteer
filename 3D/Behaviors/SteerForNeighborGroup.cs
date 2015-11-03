using System.Collections.Generic;
using UnityEngine;
using UnitySteer.Attributes;

namespace UnitySteer.Behaviors
{
    /// <summary>
    /// Steering behavior which goes through all SteerForNeighbor behaviors
    /// attached to the object and calls their CalculateNeighborContribution
    /// method for each neighbor.
    /// 
    /// This behavior will return a pure direction vector, which is the normalized
    /// aggregation of the force vectors of each of the SteerForNeigbhors descendants
    /// that are attached to the game object. These *are* affected by the
    /// steering's weight in relation to the others, but the final resulting
    /// force depends entirely on the weight of the SteerForNeighborGroup 
    /// behavior.
    /// </summary>
    /// <remarks>
    /// Previous versions of SteerFor Neighbors used to take into account separate
    /// values for filtering if a boid was an neighbor or not, which added flexibility
    /// but had the downside that we needed to evaluate the distance and alignment 
    /// of every potential neighbor as many times as we had neighbor-related 
    /// behaviors.
    /// 
    /// The current implementation only takes one set of neighbor-filtering values
    /// and applies them on detection to other vehicles in order to filter them
    /// before passing them to the attached SteerForNeighbor behaviors for them
    /// to calculate the contribution for each - on a typical boid scenario, it 
    /// cuts the checks down to a third.
    /// </remarks>
    [AddComponentMenu("UnitySteer/Steer/... for Neighbor Group")]
    [RequireComponent(typeof (Radar))]
    public class SteerForNeighborGroup : Steering
    {
        #region Private properties

        [SerializeField] private float _minRadius = 3f;
        [SerializeField] private float _maxRadius = 7.5f;

        [SerializeField] private bool _drawNeighbors = false;

        /// <summary>
        /// Angle cosine for detected vehicle heading comparison
        /// </summary>
        /// <remarks>
        /// Detected vehicles are evaluated in terms of orientation difference 
        /// and distance from the owner vehicle.  Any vehicle closer than the
        /// minimum radius is considered to be definitely a neighbor; any vehicle
        /// beyond the maxium radius is disregarded.  Vehicles between both the
        /// minimum and maximum radius are evaluated for their heading, and if
        /// the difference in angles is lower than the one specified by the 
        /// behavior's angleCos, they are considered neighbors.
        /// </remarks>
        [SerializeField, AngleCosine(0, 360)] private float _angleCos = 0.7f;

        private SteerForNeighbors[] _behaviors;


        private readonly List<Vehicle> _neighbors = new List<Vehicle>(20);

        #endregion

        #region Public properties

        /// <summary>
        /// Cosine of the maximum angle
        /// </summary>
        /// <remarks>All boid-like behaviors have an angle that helps limit them.
        /// We store the cosine of the angle for faster calculations
        /// </remarks>
        public float AngleCos
        {
            get { return _angleCos; }
            set { _angleCos = Mathf.Clamp(value, -1.0f, 1.0f); }
        }

        /// <summary>
        /// Degree accessor for the angle
        /// </summary>
        /// <remarks>The cosine is actually used in calculations for performance reasons</remarks>
        public float AngleDegrees
        {
            get { return OpenSteerUtility.DegreesFromCos(_angleCos); }
            set { _angleCos = OpenSteerUtility.CosFromDegrees(value); }
        }


        /// <summary>
        /// Minimum radius in which another vehicle is definitely considered to be a neighbor,
        /// regarding of their relative orientation to the that owns this behavior.
        /// </summary>
        public float MinRadius
        {
            get { return _minRadius; }
            set { _minRadius = value; }
        }

        /// <summary>
        /// Maximum radius for vehicles to be considered neighbors. Any vehicle beyond this
        /// range will be disregarded for neighbor calculations.
        /// </summary>
        public float MaxRadius
        {
            get { return _maxRadius; }
            set { _maxRadius = value; }
        }

        /// <summary>
        /// List of detected neighbors. I could have just returned a IEnumerable to 
        /// effectively make it read-only, but would rather give the caller the
        /// chance to avoid allocations.
        /// </summary>
        public List<Vehicle> Neighbors
        {
            get { return _neighbors; }
        }

        #endregion

        #region Methods

        protected override void Start()
        {
            base.Start();
            _behaviors = GetComponents<SteerForNeighbors>();
            foreach (var b in _behaviors)
            {
                // Ensure UnitySteer does not call them
                b.enabled = false;
                // ... and since Unity may not call them either, initialize them ourselves.
                b.Initialize();
            }
            Vehicle.Radar.OnDetected += HandleDetection;
        }

        private void HandleDetection(Radar radar)
        {
            /*
		 * Neighbors are cached on radar detection.
		 * 
		 * This means that IsInNeighborhood is evaluated when 
		 * detected, not every time that the behavior is going to 
		 * calculate its forces.  
		 * 
		 * This helps in lowering the processing load, but could 
		 * lead to a case where a vehicle is beyond the set parameters 
		 * but still considered a neighbor.
		 * 
		 * If this is a concern, make sure that vehicles are detected
		 * as often as the vehicle updates is forces.
		 * 
		 */

            _neighbors.Clear();
            // I'd prefer an iterator, but trying to cut down on allocations
            for (var i = 0; i < radar.Vehicles.Count; i++)
            {
                var other = radar.Vehicles[i];
                if (Vehicle.IsInNeighborhood(other, MinRadius, MaxRadius, AngleCos))
                {
                    _neighbors.Add(other);
                }
            }
        }

        protected override Vector3 CalculateForce()
        {
            // steering accumulator and count of neighbors, both initially zero
            var steering = Vector3.zero;
            Profiler.BeginSample("SteerForNeighborGroup.Looping over neighbors");
            // I'd prefer an iterator, but trying to cut down on allocations
            for (var i = 0; i < _neighbors.Count; i++)
            {
                var other = _neighbors[i];
                if (other == null || other.Equals(null))
                {
                    // Disregard destroyed neighbors we haven't cleared yet
                    continue;
                }
                var direction = other.Position - Vehicle.Position;
                if (!other.GameObject.Equals(null)) // Could be if the object was destroyed
                {
                    if (_drawNeighbors)
                    {
                        Debug.DrawLine(Vehicle.Position, other.Position, Color.magenta);
                    }
                    Profiler.BeginSample("SteerForNeighborGroup.Adding");
                    for (var bi = 0; bi < _behaviors.Length; bi++)
                    {
                        var behavior = _behaviors[bi];
                        if (behavior.IsDirectionInRange(direction))
                        {
                            steering += behavior.CalculateNeighborContribution(other) * behavior.Weight;
                        }
                    }
                    Profiler.EndSample();
                }
            }
            ;
            Profiler.EndSample();

            Profiler.BeginSample("Normalizing");
            // Normalize for pure direction
            steering.Normalize();
            Profiler.EndSample();

            return steering;
        }

        #endregion
    }
}