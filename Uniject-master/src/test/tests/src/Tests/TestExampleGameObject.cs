using Ninject;
using NUnit.Framework;
using System;
using UnityEngine;

namespace Tests {

    [TestFixture]
    public class TestExampleGameObject : BaseInjectedTest {

        /// <summary>
        /// Our testable example should create exactly two game objects; one for the component itself,
        /// and another for its injected prefab.
        /// </summary>
        [Test]
        public void testTestableSceneObjectCreationCount() {
            kernel.Get<TestableExample>();
            step(1); // Must step a frame to ensure our test updatable manager tracks all objects.
            Assert.AreEqual(2, kernel.Get<TestUpdatableManager>().Count);
        }

        [Test]
        public void testTestableExampleObjectResetsToOriginBelow100Metres() {
            TestableExample example = kernel.Get<TestableExample>();

            example.Obj.transform.Position = new Vector3(0, -101, 0);
            step(1);
            Assert.AreEqual(Vector3.zero, example.Obj.transform.Position);
        }
    }
}

