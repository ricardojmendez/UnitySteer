using Ninject;
using NUnit.Framework;
using System;
using UnityEngine;

namespace Tests {
    
    [TestFixture]
    public class TestCollisions : BaseInjectedTest {

        [Test]
        public void testBouncingLightDoesNotDestroyItselfAboveThresholdHeight() {
            BouncingLight light = kernel.Get<BouncingLight>();
            step();
            Assert.IsFalse(light.Obj.destroyed);
        }

        [Test]
        public void testBouncingLightDestroysItselfBelowThresholdHeight() {
            BouncingLight light = kernel.Get<BouncingLight>();
            light.Obj.transform.Translate(new Vector3(0, BouncingLight.killThresholdY - 1, 0));
            step();
            Assert.IsTrue(!light.Obj.destroyed);
        }

        [Test]
        public void testExampleSpawnsObjects() {
            kernel.Get<TestableCollisions>();
            step(2);
            int count = objectCount;
            step(10);
            Assert.Greater(objectCount, count);
        }
    }
}