using System;
using System.Collections;
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
        private float _zoomSmoothness = 6f;
        
        [Space(10)]
        [SerializeField]
        private float _startZoom = 14f;
        [SerializeField]
        private float _minZoom = 10f;
        [SerializeField]
        private float _maxZoom = 18f;
        
        private bool _canGetInput = true;
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
                    
                    _difference = currentMagnitude - prevMagnitude;
                }
            }
        }

        private void LateUpdate()
        {
            _currentZoom = Mathf.Clamp(_currentZoom - _difference * _zoomSpeed * Time.deltaTime, _minZoom, _maxZoom);
            _camera.orthographicSize = Mathf.Lerp(_camera.orthographicSize, _currentZoom, _zoomSmoothness * Time.deltaTime);
        }

        public void ResetZoom(bool immediately, System.Action onComplete = null)
        {
            _currentZoom = _startZoom;

            if (immediately)
            {
                _camera.orthographicSize = _currentZoom;
                
                onComplete?.Invoke();
            }
            else
            {
                StartCoroutine(ZoomCoroutine());
            }
            
            return;
            
            IEnumerator ZoomCoroutine()
            {
                while (!Mathf.Approximately(_camera.orthographicSize, _currentZoom))
                {
                    _camera.orthographicSize = Mathf.Lerp(_camera.orthographicSize, _currentZoom, _zoomSmoothness * Time.deltaTime);
                    
                    yield return null;
                }
                
                onComplete?.Invoke();
            }
        }
    }
}