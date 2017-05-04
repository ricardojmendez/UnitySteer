using System;
using UnityEngine;

namespace Uniject.Impl {
    public class UnityTransform : ITransform {

        private Transform transform { get; set; }

        public UnityTransform (UnityEngine.GameObject obj) {
            this.transform = obj.transform;
        }

        public Vector3 Position {
            get { return transform.position; }
            set { transform.position = value; }
        }

        public Vector3 localScale {
            get { return transform.localScale; }
            set { transform.localScale = value; }
        }

        public Quaternion Rotation {
            get { return transform.rotation; }
            set { transform.rotation = value; }
        }

        public Vector3 Forward {
            get { return transform.forward; }
            set { transform.forward = value; }
        }

        public Vector3 Up {
            get { return transform.up; }
            set { transform.up = value;}
        }

        private ITransform actualParent;
        public ITransform Parent {
            get { return actualParent; }
            set {
                this.transform.parent = ((UnityTransform)value).transform;
                this.actualParent = value;
            }
        }

        public void Translate(Vector3 byVector) {
            transform.Translate(byVector);
        }

        public void LookAt(Vector3 point) {
            transform.LookAt(point);
        }

        public Vector3 TransformDirection(Vector3 dir) {
            return transform.TransformDirection(dir);
        }
    }
}

