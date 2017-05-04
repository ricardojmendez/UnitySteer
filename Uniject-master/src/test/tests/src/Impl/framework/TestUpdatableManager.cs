using System;
using System.Collections.Generic;
using Uniject;

namespace Tests {
    /*
     * keeps track of all our game objects for the scope of a test, so they can all be updated.
     */
    public class TestUpdatableManager {
        public void step(int numUpdates) {
            for (int t = 0; t < numUpdates; t++) {

                foreach (TestableGameObject o in toAdd) {
                    objects.Add(o);
                }
                toAdd.Clear();

                foreach (TestableGameObject o in toRemove) {
                    objects.Remove(o);
                }
                toRemove.Clear();

                foreach (TestableGameObject obj in objects) {
                    obj.Update();
                }
            }
        }

        public int Count {
            get {
                if (toAdd.Count > 0) {
                    throw new InvalidOperationException("TestUpdatableManager has additional objects pending. Call step() before Count");
                }
                return objects.Count; 
            }
        }

        private HashSet<TestableGameObject> objects = new HashSet<TestableGameObject>();
        private HashSet<TestableGameObject> toAdd = new HashSet<TestableGameObject>();
        private HashSet<TestableGameObject> toRemove = new HashSet<TestableGameObject>();

        public void RegisterGameobject(TestableGameObject obj) {
            if (objects.Contains(obj) || toAdd.Contains(obj)) {
                throw new ArgumentException("Duplicate game object");
            }
            toAdd.Add(obj);
        }

        public void UnRegisterGameobject(TestableGameObject obj) {
            if (!(objects.Contains(obj) || toAdd.Contains(obj))) {
                throw new ArgumentException("Removing non existent game object");
            }
            toRemove.Add(obj);
        }
    }
}

