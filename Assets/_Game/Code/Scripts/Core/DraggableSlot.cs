using UnityEngine;
using UnityEngine.Events;

namespace VinhLB
{
    public class DraggableSlot : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private Collider _collider;
        [SerializeField]
        private GameObject _hintGO;
        
        [Header("Settings")]
        [SerializeField]
        private DraggableItem _currentItem;
        [SerializeField]
        private DraggableSlot[] _dependantSlots;
        
        [Header("Events")]
        public UnityEvent<DraggableSlot> CurrentItemChanged;
        
        public Collider Collider => _collider;
        
        public DraggableItem CurrentItem
        {
            get => _currentItem;
            set
            {
                _currentItem = value;
                UpdateVisual();
                CurrentItemChanged?.Invoke(this);
            }
        }

        protected virtual void Awake()
        {
            for (int i = 0; i < _dependantSlots.Length; i++)
            {
                _dependantSlots[i].CurrentItemChanged.AddListener(DependantSlot_CurrentItemChanged);
            }
        }
        
        protected virtual void Start()
        {
            UpdateVisual();
            DependantSlot_CurrentItemChanged(null);
        }

        protected virtual void OnDestroy()
        {
            for (int i = 0; i < _dependantSlots.Length; i++)
            {
                _dependantSlots[i].CurrentItemChanged.RemoveListener(DependantSlot_CurrentItemChanged);
            }
        }
        
        public bool IsAllDependantSlotsFilled()
        {
            for (int i = 0; i < _dependantSlots.Length; i++)
            {
                if (_dependantSlots[i].CurrentItem == null)
                {
                    return false;
                }
            }
            
            return true;
        }
        
        private void UpdateVisual()
        {
            _hintGO.SetActive(_currentItem == null);
        }

        private void DependantSlot_CurrentItemChanged(DraggableSlot slot)
        {
            if (IsAllDependantSlotsFilled())
            {
                _collider.enabled = true;
            }
            else
            {
                _collider.enabled = false;
            }
        }
    }
}