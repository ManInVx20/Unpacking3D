using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

namespace VinhLB
{
    public abstract class DraggableItem : MonoBehaviour, IDraggable
    {
        public static Transform TransformToFollowRotation;
        
        private static float TargetRotationAngleY => 
            TransformToFollowRotation != null ? -TransformToFollowRotation.localEulerAngles.y : 45f;
        
        [Header("References")]
        [SerializeField]
        protected Rigidbody _rigidbody;
        [SerializeField]
        protected Collider _collider;
        [SerializeField]
        protected MeshRenderer _meshRenderer;
        [SerializeField]
        protected Outline _outline;
        [SerializeField]
        private Transform _pivot;
        
        [Header("Settings")]
        [SerializeField]
        protected DraggableSlot _currentSlot;
        [SerializeField]
        protected DraggableSlot[] _targetSlots;
        [SerializeField]
        private SoundData _pickSoundData;
        [SerializeField]
        private SoundData _dropSoundData;
        
        [Header("Events")]
        public UnityEvent Picking;
        public UnityEvent Picked;
        public UnityEvent Dragging;
        public UnityEvent Dropping;
        public UnityEvent Dropped;
        
        private Tween _floatingTween;
        private float _yLocalPosition;
        private float _floatingCycleDuration = 1f;
        
        private Sequence _animSequence;
        private float _animDuration = 0.2f;
        private float _scaleFactor = 1.1f;
        private Vector2 _rotationRange = new Vector2(6f, 12f);
        
        public bool IsDraggable { get; set; } = true;
        public bool IsDragging { get; set; } = false;
        public Transform Pivot => _pivot;
        
        public DraggableSlot CurrentSlot
        {
            get => _currentSlot;
            set => _currentSlot = value;
        }
        
        protected virtual void Start()
        {
            if (_currentSlot == null)
            {
                _meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
                
                transform.localEulerAngles = GetRandomEulerAngles();
            }
            else
            {
                _meshRenderer.shadowCastingMode = ShadowCastingMode.On;
            }
            
            _outline.enabled = false;
        }
        
        public virtual void Pick()
        {
            IsDragging = true;
            
            Picking?.Invoke();
            
            AudioManager.Instance.PlaySound(_pickSoundData);
            
            _outline.enabled = true;
            
            if (_floatingTween.IsActive())
            {
                _floatingTween.Kill();
            }
            
            if (_animSequence.IsActive())
            {
                _animSequence.Kill();
            }
            
            _animSequence = DOTween.Sequence();
            _animSequence.Append(transform.DOScale(_scaleFactor, _animDuration));
            _animSequence.Join(transform.DOLocalRotate(new Vector3(0, TargetRotationAngleY, 0), _animDuration));
            _animSequence.OnComplete(() => Picked?.Invoke());
        }
        
        public virtual void Drag()
        {
            Dragging?.Invoke();
        }
        
        public virtual void Drop()
        {
            Dropping?.Invoke();
            
            AudioManager.Instance.PlaySound(_dropSoundData);
            
            _outline.enabled = false;
            
            if (_animSequence.IsActive())
            {
                _animSequence.Kill();
            }
            
            _animSequence = DOTween.Sequence();
            _animSequence.Append(transform.DOScale(1f, _animDuration));

            if (_currentSlot == null)
            {
                _animSequence.Join(transform.DOLocalRotate(GetRandomEulerAngles(), _animDuration));
            }
            else
            {
                if (transform.localEulerAngles != Vector3.zero)
                {
                    _animSequence.Join(transform.DOLocalRotate(Vector3.zero, _animDuration));
                }
                
                if (transform.localPosition != Vector3.zero)
                {
                    _animSequence.Join(transform.DOLocalMove(Vector3.zero, _animDuration));
                }
            }
            
            _animSequence.OnComplete(EndDrop);
        }
        
        public bool HasSlotInTarget(DraggableSlot slot)
        {
            for (int i = 0; i < _targetSlots.Length; i++)
            {
                if (slot == _targetSlots[i])
                {
                    return true;
                }
            }
            
            return false;
        }
        
        public bool DroppedInPlace()
        {
            return HasSlotInTarget(_currentSlot);
        }
        
        public Tween PlayFloatingAnim()
        {
            _yLocalPosition = transform.localPosition.y;
            
            _floatingTween =  transform.DOLocalMoveY(_yLocalPosition + 0.5f, _floatingCycleDuration)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
            
            return _floatingTween;
        }
        
        private void EndDrop()
        {
            if (_currentSlot == null)
            {
                PlayFloatingAnim();
            }
            else
            {
                if (transform.localPosition != Vector3.zero)
                {
                    transform.localPosition = Vector3.zero;
                }

                if (transform.localEulerAngles != Vector3.zero)
                {
                    transform.localEulerAngles = Vector3.zero;
                }
                
                _meshRenderer.shadowCastingMode = ShadowCastingMode.On;
            }
            
            IsDragging = false;
            
            Dropped?.Invoke();
        }
        
        private Vector3 GetRandomEulerAngles()
        {
            float randomAngleX = Random.Range(_rotationRange.x, _rotationRange.y);
            randomAngleX *= (Random.Range(0, 2) > 0 ? 1 : -1);
            
            float randomAngleZ = Random.Range(_rotationRange.x, _rotationRange.y);
            randomAngleZ *= (Random.Range(0, 2) > 0 ? 1 : -1);
            
            return new Vector3(randomAngleX, TargetRotationAngleY, randomAngleZ);
        }
    }
}