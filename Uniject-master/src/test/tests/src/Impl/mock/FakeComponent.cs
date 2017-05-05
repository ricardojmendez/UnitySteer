using System;
using Uniject;

namespace Tests {
    public class FakeComponent : TestableComponent {
        public TestableGameObject obj;

        public FakeComponent(TestableGameObject obj) : base(obj) {
            this.obj = obj;
        }


    }
}

