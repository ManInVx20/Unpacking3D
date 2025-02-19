using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace VinhLB
{
    public class IsometricCameraZoom : BaseController
    {
        [SerializeField]
        private Camera _camera;
        [SerializeField]
        private LayerMask _blockLayer;
        
        [Space(10)]
        [SerializeField]
        private float _zoomSpeed = 20f;
        [SerializeField]
        private float _zoomSmoothness = 5f;
        
        [Space(10)]
        [SerializeField]
        private float _startZoom = 14f;
        [SerializeField]
        private float _minZoom = 10f;
        [SerializeField]
        private float _maxZoom = 18f;
        
        private float _difference;
        private float _currentZoom;

        private void Awake()
        {
            ResetZoom(true);
        }

        private void Update()
        {
            if (!IsActive)
            {
                return;
            }
            
            if (VinhLBUtility.IsOnEditor())
            {
                _difference = Input.mouseScrollDelta.y; 
            }
            else
            {
                if (Input.touchCount == 2)
                {
                    Touch touchZero = Input.GetTouch(0);
                    Touch touchOne = Input.GetTouch(1);
                    
                    Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                    Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;
                    
                    float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                    float currentMagnitude = (touchZero.position - touchOne.position).magnitude;
                    
                    _difference = (currentMagnitude - prevMagnitude) * 0.01f;
                }
                else
                {
                    _difference = 0;
                }
            }
        }

        private void LateUpdate()
        {
            _currentZoom = Mathf.Clamp(_currentZoom - _difference * _zoomSpeed * Time.deltaTime, _minZoom, _maxZoom);
            _camera.orthographicSize = Mathf.Lerp(_camera.orthographicSize, _currentZoom, _zoomSmoothness * Time.deltaTime);
        }

        public Tween ResetZoom(bool immediately, float duration = 1f, System.Action onComplete = null)
        {
            _currentZoom = _startZoom;
            
            if (immediately)
            {
                _camera.orthographicSize = _currentZoom;
                
                onComplete?.Invoke();

                return null;
            }
            
            return _camera.DOOrthoSize(_currentZoom, 0.5f)
                .SetEase(Ease.InBack)
                .OnComplete(() => onComplete?.Invoke());
        }
    }
}