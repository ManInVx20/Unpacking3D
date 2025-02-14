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
                item.Dropped.AddListener(() =>
                {
                    if (item.CurrentSlot != null)
                    {
                        item.IsDraggable = false;
                        
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
            
            _cameraRotationController.ResetRotation(false, () =>
            {
                Invoke(nameof(Complete), 1f);
            });
        }
        
        private void Complete()
        {
            Debug.Log("Completed");
            AudioManager.Instance.PlaySound(_finishSoundData);
        }
    }
}