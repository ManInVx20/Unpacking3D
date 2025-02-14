using System.Collections;
using UnityEngine;

namespace VinhLB
{
    public class SoundEmitter : MonoBehaviour
    {
        [SerializeField]
        private AudioSource _audioSource;
        
        private Coroutine _playingCoroutine;
        
        public SoundData SoundData { get; private set; }
        
        public void Play()
        {
            if (_playingCoroutine != null)
            {
                StopCoroutine(_playingCoroutine);
            }
            
            _audioSource.Play();
            _playingCoroutine = StartCoroutine(WaitForSoundToEnd());
        }
        
        public void Stop()
        {
            if (_playingCoroutine != null)
            {
                StopCoroutine(_playingCoroutine);
                _playingCoroutine = null;
            }
            
            _audioSource.Stop();
            SoundManager.Instance.ReturnToPool(this);
        }
        
        public void Initialize(SoundData soundData)
        {
            SoundData = soundData;
            
            _audioSource.clip = soundData.Clip;
            _audioSource.outputAudioMixerGroup = soundData.MixerGroup;
            _audioSource.loop = soundData.Loop;
            _audioSource.playOnAwake = soundData.PlayOnAwake;
        }
        
        public void WithRandomPitch(float min = -0.05f, float max = 0.05f)
        {
            _audioSource.pitch = Random.Range(min, max);
        }
        
        private IEnumerator WaitForSoundToEnd()
        {
            yield return new WaitWhile(() => _audioSource.isPlaying);
            
            SoundManager.Instance.ReturnToPool(this);
        }
    }
}