using UnityEngine;

namespace MemoryGame.Client.Presentation.Audio
{
    public sealed class BackgroundMusicController : MonoBehaviour
    {
        [SerializeField] private AudioSource musicSource = null;

        private static BackgroundMusicController _instance;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
            PlayMusic();
        }

        private void PlayMusic()
        {
            if (musicSource == null || musicSource.clip == null || musicSource.isPlaying)
                return;

            musicSource.loop = true;
            musicSource.Play();
        }
    }
}
