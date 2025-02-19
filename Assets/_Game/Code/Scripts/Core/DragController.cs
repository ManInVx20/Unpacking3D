using System.Collections.Generic;
using UnityEngine;

namespace VinhLB
{
    public class DragController : BaseController
    {
        private class RaycastHitComparer : IComparer<RaycastHit>
        {
            public int Compare(RaycastHit x, RaycastHit y)
            {
                return x.distance.CompareTo(y.distance);
            }
        }
        
        [Header("References")]
        [SerializeField]
        private Camera _camera;
        [SerializeField]
        private Transform _backItemHolderTF;
        [SerializeField]
        private Transform _frontItemHolderTF;
        
        [Header("Settings")]
        [SerializeField]
        private Vector2Int _referenceResolution = new Vector2Int(1080, 1920);
        [SerializeField]
        private RectOffset _dragLimitOffset;
        [SerializeField]
        private LayerMask _draggableLayer;
        [SerializeField]
        private LayerMask _slotLayer;
        [SerializeField]
        private bool _blockedByWall = true;
        [SerializeField]
        private LayerMask _wallLayer;
        
        private float _leftOffset, _rightOffset, _topOffset, _bottomOffset;
        private Vector3 _dragOffset;
        private Vector3 _lastWorldPosition;
        
        private DraggableItem _pickedItem;
        
        private readonly RaycastHit[] _cachedHits = new RaycastHit[10];
        private readonly RaycastHitComparer _hitComparer = new RaycastHitComparer();
        
        private void Update()
        {
            if (!IsActive)
            {
                return;
            }
            
            if (_pickedItem == null)
            {
                if (VinhLBUtility.IsPointerDown())
                {
                    if (VinhLBUtility.IsPointerOnUI())
                    {
                        return;
                    }
                    
                    DetectAndPickItem();
                }
            }
            
            if (_pickedItem != null)
            {
                if (VinhLBUtility.IsPointerUp())
                {
                    DropItem();
                }
                else
                {
                    DragItem();
                }
            }
        }

        public override void SetActive(bool value)
        {
            base.SetActive(value);

            if (_pickedItem != null)
            {
                DropItem();
            }
        }

        private void DetectAndPickItem()
        {
            if (!VinhLBUtility.TryCastRay(_camera, out RaycastHit hitInfo, float.PositiveInfinity, _draggableLayer))
            {
                return;
            }
            
            if (CachePool.TryGetItem(hitInfo.transform, out DraggableItem draggableItem) && draggableItem.IsDraggable)
            {
                InUse = true;
                
                _pickedItem = draggableItem;
                
                _pickedItem.transform.SetParent(_frontItemHolderTF, false);
                
                _pickedItem.Pick();
                
                Vector3 pickedItemToScreenPosition = _camera.WorldToScreenPoint(_pickedItem.transform.position);
                Vector3 pointerScreenPosition = VinhLBUtility.GetPointerScreenPosition();
                _dragOffset = pickedItemToScreenPosition - pointerScreenPosition;
                _dragOffset.z = 0;
            }
        }
        
        private void DragItem()
        {
            Vector3 screenPosition = VinhLBUtility.GetPointerScreenPosition();
            screenPosition.z = _camera.WorldToScreenPoint(_pickedItem.transform.position).z;
            screenPosition += _dragOffset;
            
            (_leftOffset, _rightOffset, _topOffset, _bottomOffset) = CalculateRelativeOffset();
            screenPosition.x = Mathf.Clamp(screenPosition.x, _leftOffset, Screen.width - _rightOffset);
            screenPosition.y = Mathf.Clamp(screenPosition.y, _bottomOffset, Screen.height - _topOffset);
            
            Vector3 worldPosition = _camera.ScreenToWorldPoint(screenPosition);
            
            _pickedItem.transform.position = worldPosition;
            
            _pickedItem.Drag();
        }
        
        private void DropItem()
        {
            if (TryFindSlot(out DraggableSlot slot))
            {
                _pickedItem.CurrentSlot = slot;
                slot.CurrentItem = _pickedItem;
            
                _pickedItem.transform.SetParent(slot.transform);
            }
            else
            {
                _pickedItem.transform.SetParent(_backItemHolderTF, false);
            }
            
            _pickedItem.Drop();
            
            _pickedItem = null;
            
            InUse = false;
        }
        
        private bool TryFindSlot(out DraggableSlot slot)
        {
            slot = null;
            
            int hitCount = Physics.RaycastNonAlloc(_pickedItem.PivotTF.position, _camera.transform.forward, 
                _cachedHits, float.PositiveInfinity, _slotLayer | _wallLayer);
            if (hitCount == 0)
            {
                return false;
            }
            
            System.Array.Sort(_cachedHits, 0, hitCount, _hitComparer);
            
            for (int i = 0; i < hitCount; i++)
            {
                if (_blockedByWall)
                {
                    if (i == 0 && _wallLayer.ContainsLayer(_cachedHits[i].transform.gameObject.layer))
                    {
                        // Debug.Log($"Blocked by: {_cachedHits[i].transform.name}");
                        return false;
                    }
                }
                
                if (CachePool.TryGetSlot(_cachedHits[i].transform, out DraggableSlot checkingSlot))
                {
                    if (_pickedItem.HasSlotInTarget(checkingSlot))
                    {
                        slot = checkingSlot;
                        
                        return true;
                    }
                }
            }

            return false;
        }
        
        private (float left, float right, float top, float bottom) CalculateRelativeOffset()
        {
            int screenWidth = Screen.width;
            int screenHeight = Screen.height;

            return ((float)_dragLimitOffset.left / _referenceResolution.x * screenWidth,
                (float)_dragLimitOffset.right / _referenceResolution.x * screenWidth,
                (float)_dragLimitOffset.top / _referenceResolution.y * screenHeight,
                (float)_dragLimitOffset.bottom / _referenceResolution.y * screenHeight);
        }
    }
}