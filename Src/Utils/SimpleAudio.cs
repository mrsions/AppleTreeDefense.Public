using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;

namespace ATD
{
    public class SimpleAudio : MonoBehaviour
    {
        public static SimpleAudio Instance;

        public static void Play(AudioClip? clip, float volume = 1f)
        {
            if (clip)
            {
                Instance.audioSource.PlayOneShot(clip, volume);
            }
        }

        public AudioSource audioSource;

        private void OnEnable()
        {
            Instance = this;
            audioSource = GetComponent<AudioSource>();
        }

        private void OnDisable()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }
    }
}