using System;
using System.Collections.Generic;
using Uniject;
using UnityEngine;

namespace Tests {

    public class NullTransformException : Exception {
    }

    public class FakeGameObject : TestableGameObject {

        public class FakeTransform : ITransform {
   
            public bool active { get; set; }

            public Vector3 Position { get; set; }

            public Vector3 localScale { get; set; }

            public Quaternion Rotation { get; set; }

            public Vector3 Forward { get; set; }

            public Vector3 Up { get; set; }

            public Vector3 TransformDirection(Vector3 dir) {
                return dir;
            }

            public void LookAt(Vector3 point) {
            }

            private ITransform t;

            public ITransform Parent {
                get {
                    return t;
                }
                set {
                    if (value == null) {
                        throw new NullTransformException ();
                    }

                    if (t == value) {
                        throw new ArgumentException("Cannot assign a transform's parent to itself");
                    }

                    t = value;
                }
            }

            public void Translate(Vector3 byVector) {
                this.Position += byVector;
            }
        }

        private TestUpdatableManager manager;

        public FakeGameObject(ITransform transform, TestUpdatableManager manager) : base(transform) {
            this.manager = manager;
            manager.RegisterGameobject(this);
            active = true;
        }

        public override void Destroy() {
            base.Destroy();
            manager.UnRegisterGameobject(this);
        }

        public override string name { get; set; }

        public override bool active { get; set; }

        public override void setActiveRecursively(bool active) {
            if (destroyed) {
                throw new Exception ("Cannot access destroyed gameobject.");
            }
            active = false;
        }

        public override int layer { get; set; }
    }
}

