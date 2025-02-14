using UnityEngine;

namespace VinhLB
{
    public class IsometricCameraZoom : MonoBehaviour
    {
        [SerializeField]
        private Camera _camera;
        [SerializeField]
        private float _zoomSpeed = 6f;
        [SerializeField]
        private float _zoomSmoothness = 5f;
        [SerializeField]
        private float _minZoom = 2f;
        [SerializeField]
        private float _maxZoom = 40f;
        
        private float _currentZoom;

        private void Update()
        {
            _currentZoom = Mathf.Clamp(_currentZoom - Input.mouseScrollDelta.y * _zoomSpeed * Time.deltaTime, -_minZoom, _maxZoom);
            _camera.orthographicSize = Mathf.Lerp(_camera.orthographicSize, _currentZoom, _zoomSmoothness * Time.deltaTime);
        }
    }
}