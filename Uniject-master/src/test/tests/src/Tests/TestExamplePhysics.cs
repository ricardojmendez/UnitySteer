using Ninject;
using NUnit.Framework;
using System;
using UnityEngine;

namespace Tests {

    [TestFixture]
    public class TestExamplePhysics : BaseInjectedTest {

        [Test]
        public void testRaycastScene() {
            kernel.Get<ScanningLaser>();
            step(10);
        }
    }
}

