using UnityEngine;

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
        
        public DraggableItem CurrentItem
        {
            get => _currentItem;
            set
            {
                _currentItem = value;
                UpdateVisual();
            }
        }
        
        private void UpdateVisual()
        {
            _hintGO.SetActive(_currentItem == null);
        }
    }
}