using System;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using UnityEngine;

namespace UnityTest
{
    [TestFixture]
    [Category("UnitTests")]
    internal class UnitTests
    {
        [Test]
        public void IsOddRangeTest([NUnit.Framework.Range(1, 10, 2)] int x)
        {
            if (x % 2 == 1)
            {
                Assert.Pass();
            }
            else
            {
                Assert.Fail();
            }
        }

        [Test]
        public void GetAnglePositive()
        {
            Assert.AreEqual(45, Vector2.Angle(Vector2.right, new Vector2(1,1)));
        }

        [Test]
        public void GetAngleNegative()
        {
            Assert.AreEqual(-45, Vector2.Angle(Vector2.right, new Vector2(1, -1)));
        }
    }
}
