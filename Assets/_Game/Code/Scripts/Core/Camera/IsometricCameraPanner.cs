using System;
using UnityEngine;

namespace VinhLB
{
    public class IsometricCameraPanner : MonoBehaviour
    {
        [SerializeField]
        private Camera _camera;
        [SerializeField]
        private float _panSpeed = 10f;
        [SerializeField]
        private Vector2 _panLimitX;
        [SerializeField]
        private Vector2 _panLimitZ;

        private void Update()
        {
            Vector2 pointerScreenPosition = VinhLBUtility.GetPointerScreenPosition();
            
            transform.position += Quaternion.Euler(0, _camera.transform.eulerAngles.y, 0) * 
                                  new Vector3(pointerScreenPosition.x, 0, pointerScreenPosition.y) * 
                                  (_panSpeed * Time.deltaTime);
            
            Vector3 clampedPosition = transform.position;
            clampedPosition.x = Mathf.Clamp(clampedPosition.x, _panLimitX.x, _panLimitX.y);
            clampedPosition.z = Mathf.Clamp(clampedPosition.z, _panLimitZ.x, _panLimitZ.y);
            transform.position = clampedPosition;
        }
    }
}