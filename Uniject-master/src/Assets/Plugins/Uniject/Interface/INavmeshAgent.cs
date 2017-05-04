using System;
using UnityEngine;

namespace Uniject {
	public interface INavmeshAgent {
		void setDestination(Vector3 dest);
        void Stop();
		void setSpeedMultiplier(float multiplier);
		void onPlacedOnNavmesh();
		bool Enabled { get; set; }
        float radius { get; set; }
        float BaseOffset { get; set; }
        ObstacleAvoidanceType obstacleAvoidanceType { get; set; }
	}
}

