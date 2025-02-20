using System.Collections;
using DG.Tweening;
using EmeraldPowder.CameraScaler;
using UnityEngine;

namespace VinhLB
{
    public class IsometricCameraZoom : BaseController
    {
        [SerializeField]
        private Camera _camera;
        [SerializeField]
        private CameraScaler _cameraScaler;
        [SerializeField]
        private LayerMask _blockLayer;
        
        [Space(10)]
        [SerializeField]
        private float _zoomSpeed = 20f;
        [SerializeField]
        private float _zoomSmoothness = 5f;
        
        [Space(10)]
        [SerializeField]
        private float _startZoomSize = 1f;
        [SerializeField]
        private float _minZoomSize = 0.8f;
        [SerializeField]
        private float _maxZoomSize = 1.2f;
        
        private float _difference;
        private float _currentZoomSize;

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
            _currentZoomSize = Mathf.Clamp(_currentZoomSize + _difference * _zoomSpeed * Time.deltaTime, _minZoomSize, _maxZoomSize);
            _cameraScaler.CameraZoom = Mathf.Lerp(_cameraScaler.CameraZoom, _currentZoomSize, _zoomSmoothness * Time.deltaTime);
        }
        
        public Tween ResetZoom(bool immediately, float duration = 1f, System.Action onComplete = null)
        {
            _currentZoomSize = _startZoomSize;
            
            if (immediately)
            {
                _cameraScaler.CameraZoom = _currentZoomSize;
                
                onComplete?.Invoke();

                return null;
            }
            
            return DOTween.To(() => _cameraScaler.CameraZoom, (value) => _cameraScaler.CameraZoom = value, 
                    _currentZoomSize, duration)
                .SetEase(Ease.InBack)
                .OnComplete(() => onComplete?.Invoke());
        }
    }
}