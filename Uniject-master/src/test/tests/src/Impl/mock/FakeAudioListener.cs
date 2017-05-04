using System;
using Uniject;

namespace Tests {
    public class FakeAudioListener : TestableComponent, IAudioListener {

        public FakeAudioListener(TestableGameObject obj) : base(obj) {
        }

        public void noOp() {
        }
    }
}

