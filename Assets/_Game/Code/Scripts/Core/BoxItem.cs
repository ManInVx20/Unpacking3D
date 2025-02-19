using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace VinhLB
{
    public class BoxItem : MonoBehaviour, IInteractable, IPointerClickHandler
    {
        [SerializeField]
        private DraggableItem[] _innerItems;
        [SerializeField]
        private Transform _itemHolderTF;
        [SerializeField]
        private Transform _bottomLeftSpawnPoint;
        [SerializeField]
        private Transform _topRightSpawnPoint;
        [SerializeField]
        private SoundData _interactSoundData;
        
        private Stack<DraggableItem> _availableInnerItemStack;
        private Sequence _boxSequence;
        private Vector3 _startScale;
        
        public DraggableItem[] InnerItems => _innerItems;
        public bool HasAnyItem => _availableInnerItemStack.Count > 0;
        
        public bool IsInteractable { get; set; } = true;
        
        private void Awake()
        {
            // Put items to the box 
            for (int i = 0; i < _innerItems.Length; i++)
            {
                _innerItems[i].transform.SetParent(transform);
                _innerItems[i].transform.localPosition = Vector3.zero;
                
                _innerItems[i].gameObject.SetActive(false);
            }
            
            // Create stack to pop item easily
            _innerItems.Shuffle();
            _availableInnerItemStack = new Stack<DraggableItem>(_innerItems);
            
            _startScale = transform.localScale;
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (!IsInteractable)
            {
                return;
            }
            
            if (VinhLBUtility.IsPointerOnUI())
            {
                return;
            }
            
            Interact();
        }
        
        public void Interact()
        {
            if (!HasAnyItem)
            {
                Debug.Log("No item available");
                return;
            }

            // Get and pull item
            DraggableItem item = _availableInnerItemStack.Pop();
            item.IsDraggable = false;
            item.gameObject.SetActive(true);
            item.transform.SetParent(_itemHolderTF);
            
            float animDuration = 0.5f;
            Sequence itemSequence = DOTween.Sequence();
            itemSequence.Append(item.transform.DOLocalJump(GetRandomSpawnPosition(), 1f, 1, animDuration)
                .SetEase(Ease.OutSine));
            itemSequence.Join(item.transform.DOScale(item.transform.localScale, animDuration)
                .From(item.transform.localScale * 0.5f)
                .SetEase(Ease.OutSine));
            itemSequence.OnComplete(() =>
            {
                item.PlayFloatingAnim();
                item.IsDraggable = true;
            });
            
            // Play sound and animation when interacting
            AudioManager.Instance.PlaySound(_interactSoundData);
            
            if (_boxSequence.IsActive())
            {
                _boxSequence.Kill();
            }
            
            _boxSequence = DOTween.Sequence();
            transform.localScale = _startScale;
            _boxSequence.Append(transform.DOPunchScale(Vector3.one * 0.1f, animDuration, 6));
            
            if (!HasAnyItem)
            {
                IsInteractable = false;
                
                // _boxSequence.AppendInterval(0.5f);
                _boxSequence.Append(transform.DOScale(0, animDuration).SetEase(Ease.InBack));
                _boxSequence.OnComplete(() => gameObject.SetActive(false));
            }
        }
        
        private Vector3 GetRandomSpawnPosition()
        {
            return new Vector3(
                Random.Range(_bottomLeftSpawnPoint.localPosition.x, _topRightSpawnPoint.localPosition.x),
                Random.Range(_bottomLeftSpawnPoint.localPosition.y, _topRightSpawnPoint.localPosition.y),
                0);
        }
        
#if UNITY_EDITOR
        [ContextMenu(nameof(CollectDraggableItems))]
        private void CollectDraggableItems()
        {
            DraggableItem[] items = FindObjectsByType<DraggableItem>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            _innerItems = items;
        }
#endif
    }
}