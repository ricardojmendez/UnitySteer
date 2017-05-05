using System;
using UnityEngine;

namespace Uniject.Impl {
    public class UnitySphereCollider : ISphereCollider {
        private SphereCollider collider;
        private UnityPhysicsMaterial mat;

        public UnitySphereCollider(GameObject obj) {
            this.collider = obj.AddComponent<SphereCollider>();
        }

        public float radius {
            get { return collider.radius; }
            set { collider.radius = value; }
        }

        public bool enabled {
            get { return collider.enabled; }
            set { collider.enabled = value; }
        }

        public Vector3 center {
            get { return collider.center; }
            set { collider.center = value; }
        }

        public IPhysicMaterial material {
            get { return mat; }
            set {
                mat = (UnityPhysicsMaterial) value;
                collider.material = mat.material;
            }
        }
    }
}

