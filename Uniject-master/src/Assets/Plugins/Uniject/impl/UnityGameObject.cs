using System.Collections.Generic;
using System;
using UnityEngine;
using Uniject;

namespace Uniject.Impl {
    public class UnityGameObject : TestableGameObject {

        public GameObject obj { get; private set; }
        public UnityGameObject (GameObject obj) : base(new UnityTransform(obj)) {
            this.obj = obj;
            obj.AddComponent<UnityGameObjectBridge>().wrapping = this;
        }

        public override void Destroy() {
            base.Destroy();
            GameObject.Destroy (this.obj);
        }

        public override bool active {
            get { return obj.active; }
            set { obj.active = value; }
        }

        public override string name {
            get { return obj.name; }
            set { obj.name = value; }
        }

        public override void setActiveRecursively(bool active) {
            obj.SetActiveRecursively(active);
        }

        public override int layer {
            get { return obj.layer; }
            set { obj.layer = value; }
        }
    }
}
