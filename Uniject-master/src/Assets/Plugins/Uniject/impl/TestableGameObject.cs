using System;
using System.Collections.Generic;
using UnityEngine;
using Uniject;

namespace Uniject {

    /// <summary>
    /// A testable equivalent of <c>UnityEngine.GameObject</c>.
    /// </summary>
    public abstract class TestableGameObject {

        private List<TestableComponent> components = new List<TestableComponent>();

        public ITransform transform { get; private set; }

        public TestableGameObject(ITransform transform) {
            this.transform = transform;
        }

        public void registerComponent (Uniject.TestableComponent component) {
            components.Add(component);
        }

        public bool destroyed { get; private set; }

        public virtual void Destroy () {
            if (!destroyed) {
                foreach (TestableComponent component in this.components) {
                    component.OnDestroy();
                }
                destroyed = true;
            }
        }

        public void Update() {
            if (active) {
                for (int t = 0; t < components.Count; t++) {
                    TestableComponent component = components[t];
                    component.OnUpdate();
                }
            }
        }

        public T getComponent<T>() where T : class {
            for (int t = 0; t < components.Count; t++) {
                TestableComponent component = components[t];
                if (component is T) {
                    return component as T;
                }
            }

            return null;
        }

        public void OnCollisionEnter(Collision c) {
            for (int t = 0; t < components.Count; t++) {
                components[t].OnCollisionEnter(c);
            }
        }

        public abstract void setActiveRecursively(bool active);
        public abstract bool active { get; set; }
        public abstract string name { get; set; }
        public abstract int layer { get; set; }
    }
}
