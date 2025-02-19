using DG.Tweening;
using UnityEngine;

namespace VinhLB
{
    public class Level : MonoBehaviour
    {
        [SerializeField]
        private DragController _dragController;
        [SerializeField]
        private IsometricCameraRotation _cameraRotationController;
        [SerializeField]
        private IsometricCameraZoom _cameraZoomController;
        
        [SerializeField]
        private Transform _cameraPivot;
        [SerializeField]
        private BoxItem _boxItem;
        
        [SerializeField]
        private SoundData _correctSoundData;
        [SerializeField]
        private SoundData _finishSoundData;
        
        private void Awake()
        {
            CachePool.Clear();
            
            DraggableItem.TransformToFollowRotation = _cameraPivot;

            foreach (DraggableItem item in _boxItem.InnerItems)
            {
                item.Dropping.AddListener(() =>
                {
                    if (item.CurrentSlot != null)
                    {
                        item.IsDraggable = false;
                        item.SetColliderEnabled(false);
                    }
                });
                item.Dropped.AddListener(() =>
                {
                    if (item.CurrentSlot != null)
                    {
                        AudioManager.Instance.PlaySound(_correctSoundData);
                        EffectManager.Instance.GetAndPlay(item.transform.position);
                    }
                    
                    CheckCompletion();
                });
            }
        }
        
        private void Start()
        {
            _dragController.SetActive(true);
            _cameraRotationController.SetActive(true);
            _cameraZoomController.SetActive(true);
            
            for (int i = 0; i < _boxItem.InnerItems.Length; i++)
            {
                IInteractable interactableItem = _boxItem.InnerItems[i] as IInteractable;
                Debug.Log($"{_boxItem.InnerItems[i].gameObject.name}: {interactableItem != null}");
            }
        }
        
        private void CheckCompletion()
        {
            for (int i = 0; i < _boxItem.InnerItems.Length; i++)
            {
                if (!_boxItem.InnerItems[i].DroppedInPlace())
                {
                    return;
                }
            }
            
            _dragController.SetActive(false);
            _cameraRotationController.SetActive(false);
            _cameraZoomController.SetActive(false);
            
            Sequence sequence = DOTween.Sequence();
            sequence.AppendInterval(1f);
            sequence.Append(_cameraRotationController.ResetRotation(false, 0.5f));
            sequence.Join(_cameraZoomController.ResetZoom(false, 0.5f));
            sequence.OnComplete(Complete);
        }
        
        private void Complete()
        {
            Debug.Log("Completed");
            AudioManager.Instance.PlaySound(_finishSoundData);
        }
    }
}