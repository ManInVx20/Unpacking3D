using System.Collections;
using UnityEngine;

namespace VinhLB
{
    public class EffectEmitter : MonoBehaviour
    {
        [SerializeField]
        private ParticleSystem _particleSystem;
        
        private Coroutine _playingCoroutine;
        
        public void Play()
        {
            if (_playingCoroutine != null)
            {
                StopCoroutine(_playingCoroutine);
            }
            
            _particleSystem.Play();
            
            _playingCoroutine = StartCoroutine(WaitToEnd());
        }
        
        public void Stop()
        {
            if (_playingCoroutine != null)
            {
                StopCoroutine(_playingCoroutine);
                _playingCoroutine = null;
            }
            
            _particleSystem.Stop();
            
            EffectManager.Instance.ReturnToPool(this);
        }
        
        private IEnumerator WaitToEnd()
        {
            yield return new WaitWhile(() => _particleSystem.isPlaying);
            
            EffectManager.Instance.ReturnToPool(this);
        }
    }
}