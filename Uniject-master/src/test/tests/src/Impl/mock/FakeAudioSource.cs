using System;
using UnityEngine;
using Uniject;

namespace Tests {
    public class FakeAudioSource : TestableComponent, IAudioSource {

        public FakeAudioSource(TestableGameObject obj) : base(obj) {
        }

        public void playOneShot(AudioClip clip) {
        }

        public void loopSound(AudioClip clip) {
        }

        public void Play() {
        }

        public bool isPlaying { get; set; }
    }
}
