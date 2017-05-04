using System;
using UnityEngine;

namespace Uniject.Impl {
    public class UnityPhysicsMaterial : IPhysicMaterial {
        public UnityEngine.PhysicMaterial material { get; private set; }
        public UnityPhysicsMaterial(UnityEngine.PhysicMaterial mat) {
            this.material = mat;
        }

        public float dynamicFriction {
            get { return material.dynamicFriction; }
            set { material.dynamicFriction = value; }
        }

        public float staticFriction {
            get { return material.staticFriction; }
            set { material.staticFriction = value; }
        }


        public float bounciness {
            get { return material.bounciness; }
            set { material.bounciness = value; }
        }

        public Vector3 frictionDirection2 {
            get { return material.frictionDirection2; }
            set { material.frictionDirection2 = value; }
        }

        public float dynamicFriction2 {
            get { return material.dynamicFriction2; }
            set { material.dynamicFriction2 = value; }
        }

        public float staticFriction2 {
            get { return material.staticFriction2; }
            set { material.staticFriction2 = value; }
        }

        public PhysicMaterialCombine frictionCombine {
            get { return material.frictionCombine; }
            set { material.frictionCombine = value; }
        }

        public PhysicMaterialCombine bounceCombine {
            get { return material.bounceCombine; }
            set { material.bounceCombine = value; }
        }
    }
}

