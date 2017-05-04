using System;
using Uniject;
using UnityEngine;

namespace Tests {
    public class FakeNavmeshAgent : TestableComponent, INavmeshAgent {
        public FakeNavmeshAgent(TestableGameObject obj) : base(obj) {
        }

        public void setDestination(UnityEngine.Vector3 dest) {
        }

        public void Stop() {
        }

        public void setSpeedMultiplier(float multiplier) {
        }
    
        public void onPlacedOnNavmesh() {
        }

        public ObstacleAvoidanceType obstacleAvoidanceType { get; set; }

        public float BaseOffset { get; set; }

        public bool Enabled { get; set; }
    
        public float radius { get; set; }
    }
}

