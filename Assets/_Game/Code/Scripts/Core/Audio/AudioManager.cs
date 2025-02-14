using UnityEngine;

namespace VinhLB
{
    public class AudioManager : PersistentMonoSingleton<AudioManager>
    {
        [SerializeField]
        private AudioSource _audioSource;

        public void PlaySound(SoundData soundData)
        {
            _audioSource.PlayOneShot(soundData.Clip, soundData.Volume);
        }
    }
}