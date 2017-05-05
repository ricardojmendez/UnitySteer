using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uniject;

namespace Tests {

    /*
     * Equivalent of UnityEngine.Time that uses a configurable fixed delta time.
     * This is a component so that it gets Update() calls and can provide a pseudo realTimeSinceStartup.
     */
    [GameObjectBoundary]
    class MockTime : TestableComponent, ITime {
    
        public float DeltaTime { get; set; }

        public MockTime(TestableGameObject obj) : base(obj) {
            DeltaTime = 1.0f;
        }

        public override void Update() {
            time += DeltaTime;
        }

        private float time;

        public float realtimeSinceStartup {
            get {
                return time;
            }
            set { time = value; }
        }
    }
}
