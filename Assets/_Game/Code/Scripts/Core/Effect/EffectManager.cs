using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace VinhLB
{
    public class EffectManager : MonoSingleton<EffectManager>
    {
        [SerializeField]
        private EffectEmitter _effectEmitterPrefab;
        [SerializeField]
        private bool _collectionCheck = true;
        [SerializeField]
        private int _defaultCapacity = 10;
        [SerializeField]
        private int _maxPoolSize = 100;
        [SerializeField]
        private int _maxInstances = 30;
        
        private IObjectPool<EffectEmitter> _effectEmitterPool;
        private readonly List<EffectEmitter> _activeEffectEmitterList = new();
        
        private void Start()
        {
            InitializePool();
        }
        
        private void InitializePool()
        {
            _effectEmitterPool = new ObjectPool<EffectEmitter>(
                CreateEffect,
                OnTakeFromPool,
                OnReturnToPool,
                OnDestroyPoolObject,
                _collectionCheck,
                _defaultCapacity,
                _maxPoolSize);
        }
        
        public EffectEmitter GetAndPlay(Vector3 position)
        {
            EffectEmitter effectEmitter = _effectEmitterPool.Get();
            effectEmitter.transform.SetParent(transform);
            effectEmitter.transform.position = position;
            
            effectEmitter.Play();
            
            return effectEmitter;
        }
        
        public void ReturnToPool(EffectEmitter effectEmitter)
        {
            _effectEmitterPool.Release(effectEmitter);
        }
        
        private EffectEmitter CreateEffect()
        {
            EffectEmitter effectEmitter = Instantiate(_effectEmitterPrefab);
            effectEmitter.gameObject.SetActive(false);
            
            return effectEmitter;
        }
        
        private void OnTakeFromPool(EffectEmitter effectEmitter)
        {
            effectEmitter.gameObject.SetActive(true);
            _activeEffectEmitterList.Add(effectEmitter);
        }
        
        private void OnReturnToPool(EffectEmitter effectEmitter)
        {
            effectEmitter.gameObject.SetActive(false);
            _activeEffectEmitterList.Remove(effectEmitter);
        }
        
        private void OnDestroyPoolObject(EffectEmitter effectEmitter)
        {
            Destroy(effectEmitter.gameObject);
        }
    }
}