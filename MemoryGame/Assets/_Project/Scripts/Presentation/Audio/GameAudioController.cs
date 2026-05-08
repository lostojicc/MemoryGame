using UnityEngine;

namespace MemoryGame.Client.Presentation.Audio
{
    public sealed class GameAudioController : MonoBehaviour
    {
        [SerializeField] private AudioSource effectsSource = null;
        [SerializeField] private AudioClip flipClip = null;
        [SerializeField] private AudioClip matchClip = null;
        [SerializeField] private AudioClip mismatchClip = null;

        public void PlayFlip()
        {
            PlayOneShot(flipClip);
        }

        public void PlayMatch()
        {
            PlayOneShot(matchClip);
        }

        public void PlayMismatch()
        {
            PlayOneShot(mismatchClip);
        }

        private void PlayOneShot(AudioClip clip)
        {
            if (effectsSource == null || clip == null)
                return;

            effectsSource.PlayOneShot(clip);
        }
    }
}
