using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace VinhLB
{
    public class SoundManager : MonoSingleton<SoundManager>
    {
        [SerializeField]
        private SoundEmitter _soundEmitterPrefab;
        [SerializeField]
        private bool _collectionCheck = true;
        [SerializeField]
        private int _defaultCapacity = 10;
        [SerializeField]
        private int _maxPoolSize = 100;
        [SerializeField]
        private int _maxInstances = 30;
        
        private IObjectPool<SoundEmitter> _soundEmitterPool;
        private readonly List<SoundEmitter> _activeSoundEmitterList = new();
        
        public readonly Dictionary<SoundData, int> SoundCountDict = new();

        private void Start()
        {
            InitializePool();
        }
        
        public SoundBuilder CreateSound() => new SoundBuilder(this);
        
        public bool CanPlaySound(SoundData soundData)
        {
            if (SoundCountDict.TryGetValue(soundData, out int count))
            {
                if (count >= _maxInstances)
                {
                    return false;
                }
            }
            
            return true;
        }

        public SoundEmitter Get()
        {
            return _soundEmitterPool.Get();
        }

        public void ReturnToPool(SoundEmitter soundEmitter)
        {
            _soundEmitterPool.Release(soundEmitter);
        }
        
        private void InitializePool()
        {
            _soundEmitterPool = new ObjectPool<SoundEmitter>(
                CreateSoundEmitter,
                OnTakeFromPool,
                OnReturnToPool,
                OnDestroyPoolObject,
                _collectionCheck,
                _defaultCapacity,
                _maxPoolSize);
        }
        
        private SoundEmitter CreateSoundEmitter()
        {
            SoundEmitter soundEmitter = Instantiate(_soundEmitterPrefab);
            soundEmitter.gameObject.SetActive(false);

            return soundEmitter;
        }
        
        private void OnTakeFromPool(SoundEmitter soundEmitter)
        {
            soundEmitter.gameObject.SetActive(true);
            _activeSoundEmitterList.Add(soundEmitter);
        }
        
        private void OnReturnToPool(SoundEmitter soundEmitter)
        {
            if (SoundCountDict.TryGetValue(soundEmitter.SoundData, out int count))
            {
                SoundCountDict[soundEmitter.SoundData] -= count > 0 ? 1 : 0;
            }
            
            soundEmitter.gameObject.SetActive(false);
            _activeSoundEmitterList.Remove(soundEmitter);
        }
        
        private void OnDestroyPoolObject(SoundEmitter soundEmitter)
        {
            Destroy(soundEmitter.gameObject);
        }
    }
}