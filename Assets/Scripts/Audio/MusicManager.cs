using EuropeanWars.Core;
using System.Collections.Generic;
using UnityEngine;

namespace EuropeanWars.Audio {
    public class MusicManager : MonoBehaviour {
        public static MusicManager Singleton { get; private set; }
        public AudioClip mainTheme;
        public List<AudioClip> clips = new List<AudioClip>();
        public AudioSource audioSource;

        public List<AudioClip> effects = new List<AudioClip>();
        public AudioSource audioEffectsSource;

        private bool isCreated;
        private System.Random rng;

        public void Awake() {
            if (!isCreated) {
                DontDestroyOnLoad(gameObject);
                Singleton = this;
                isCreated = true;
                rng = new System.Random(80);
            }
        }

        private void Update() {
            if (clips.Count > 0) {
                if (!audioSource.isPlaying) {
                    if (GameInfo.gameStarted) {
                        audioSource.clip = clips[rng.Next(0, clips.Count - 1)];
                    }
                    else {
                        audioSource.clip = mainTheme;
                    }
                    audioSource.Play();
                }
            }
        }
    }
}
