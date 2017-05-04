using Ninject;
using NUnit.Framework;
using System;
using Uniject.Configuration;

namespace Tests {

    /// <summary>
    /// Tests for the XML backed configuration injection system.
    /// </summary>
    [TestFixture]
    public class TestUnijectConfig : BaseInjectedTest {

        private class HasInjectedPrimitiveTypes {
            public string val { get; private set; }
            public float f { get; private set; }
            public double d { get; private set; }
            public HasInjectedPrimitiveTypes([XMLConfigValue("xml/test", "root/string")] string val,
                                             [XMLConfigValue("xml/test", "root/float")] float f,
                                             [XMLConfigValue("xml/test", "root/float")] double d) {
                this.val = val;
                this.f = f;
                this.d = d;
            }
        }

        [Test]
        public void testInjectedConfigValue() {
            HasInjectedPrimitiveTypes has = kernel.Get<HasInjectedPrimitiveTypes>();
            Assert.AreEqual("Hello World", has.val);
            Assert.AreEqual(3.14159f, has.f);
            Assert.AreEqual(3.14159, has.d);
        }
    }
}
