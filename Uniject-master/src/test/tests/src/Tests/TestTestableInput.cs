using Moq;
using Ninject;
using NUnit.Framework;
using System;
using Uniject;
using UnityEngine;

namespace Tests {
    [TestFixture]
    public class TestTestableInput : BaseInjectedTest {

        /// <summary>
        /// If there is no input, no force should be applied to the sphere.
        /// </summary>
        [Test]
        public void testNoMovement() {
            TestableInput scene = kernel.Get<TestableInput>();
            Mock<IRigidBody> mockBody = Mock.Get(scene.sphere.body);
            step();
            mockBody.Verify(mock => mock.AddForce(It.IsAny<Vector3>()), Times.Never());
        }

        /// <summary>
        /// If positive horizontal input is given, a force should be applied to move
        /// the ball to the right.
        /// </summary>
        [Test]
        public void testMovementToRight() {
            TestableInput scene = kernel.Get<TestableInput>();
            var mockInput = getMockedDependency<IInput>();
            mockInput.Setup(mock => mock.GetAxis("Horizontal")).Returns(1);

            Mock<IRigidBody> mockBody = Mock.Get(scene.sphere.body);
            step();
            mockBody.Verify(mock => mock.AddForce(It.Is<Vector3>(v => v.x > 0)), Times.Exactly(1));
        }
    }
}
