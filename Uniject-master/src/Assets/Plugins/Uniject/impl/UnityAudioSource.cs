using System;
using Uniject;
using UnityEngine;

namespace Uniject.Impl {
    public class UnityAudioSource : IAudioSource {
    	private AudioSource source;
    	public UnityAudioSource(GameObject obj) {
            this.source = obj.GetComponent<AudioSource>();
            if (this.source == null) {
                this.source = (AudioSource)obj.AddComponent(typeof(AudioSource));
            }
            source.rolloffMode = AudioRolloffMode.Linear;
    	}

        public void loopSound(AudioClip clip) {
            source.loop = true;
            source.clip = clip;
            source.Play();
        }

        public void playOneShot(AudioClip clip) {
            source.PlayOneShot(clip);
        }

    	public void Play ()
    	{
    		source.Play();
    	}

        public bool isPlaying {
            get { return source.isPlaying; }
        }
    }
}
