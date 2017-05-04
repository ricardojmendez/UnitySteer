using System;
using UnityEngine;

namespace Uniject {
    public struct Collision {
        public Vector3 relativeVelocity { get; private set; }
        public ITransform transform { get; private set; }
        public TestableGameObject gameObject { get; private set; }
        public ContactPoint[] contacts { get; private set; }

        public Collision(Vector3 relativeVelocity,
                         ITransform transform,
                         TestableGameObject gameObject,
                         ContactPoint[] contacts) : this() {
            this.relativeVelocity = relativeVelocity;
            this.transform = transform;
            this.gameObject = gameObject;
            this.contacts = contacts;
        }
    }
}

