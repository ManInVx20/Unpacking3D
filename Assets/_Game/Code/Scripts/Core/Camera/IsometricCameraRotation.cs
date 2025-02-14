using System;
using System.Collections;
using UnityEngine;

namespace VinhLB
{
    public class IsometricCameraRotation : BaseController
    {
        [SerializeField]
        private BaseController[] _blockControllers;
        [SerializeField]
        private Camera _camera;
        [SerializeField]
        private LayerMask _blockLayer;
        
        [Space(10)]
        [SerializeField]
        private float _rotationSpeed = 4f;
        [SerializeField]
        private float _smoothSpeed = 0.04f;
        
        [Space(10)]
        [SerializeField]
        private float _startAngleY = 45f;
        [SerializeField]
        private float _leftAngleLimit = 105f;
        [SerializeField]
        private float _rightAngleLimit = -15f;
        
        private float _deltaX;
        
        private void Awake()
        {
            ResetRotation(true);
        }
        
        private void Update()
        {
            if (!IsActive)
            {
                return;
            }
            
            if (IsAnyControllerInUse(_blockControllers) || 
                !InUse && VinhLBUtility.TryCastRay(_camera, out _, float.PositiveInfinity, _blockLayer))
            {
                Stop();
                
                return;
            }
            
            if (VinhLBUtility.IsPointerActive())
            {
                InUse = true;
                
                Vector2 deltaPosition = VinhLBUtility.GetPointerDeltaPosition();
                _deltaX += deltaPosition.x * _rotationSpeed * Time.deltaTime;
                
                if (_leftAngleLimit > _rightAngleLimit)
                {
                    _deltaX = Mathf.Clamp(_deltaX, _rightAngleLimit, _leftAngleLimit);
                }
                else
                {
                    _deltaX = Mathf.Clamp(_deltaX, _leftAngleLimit, _rightAngleLimit);   
                }
                
                // transform.Rotate(Vector3.up, deltaX * _rotationSpeed * Time.deltaTime, Space.World);
            }
            else
            {
                Stop();
            }
        }

        private void LateUpdate()
        {
            // if (IsActive && !IsAnyControllerInUse(_blockControllers))
            // {
            //     if (VinhLBUtility.IsPointerActive())
            //     {
            //         InUse = true;
            //         
            //         Vector2 deltaPosition = VinhLBUtility.GetPointerDeltaPosition();
            //         _deltaX += deltaPosition.x * _rotationSpeed * Time.deltaTime;
            //         
            //         if (_leftAngleLimit > _rightAngleLimit)
            //         {
            //             _deltaX = Mathf.Clamp(_deltaX, _rightAngleLimit, _leftAngleLimit);
            //         }
            //         else
            //         {
            //             _deltaX = Mathf.Clamp(_deltaX, _leftAngleLimit, _rightAngleLimit);   
            //         }
            //     
            //         // transform.Rotate(Vector3.up, deltaX * _rotationSpeed * Time.deltaTime, Space.World);
            //     }
            //     else
            //     {
            //         Stop();
            //     }
            // }
            
            Vector3 targetEulerAngles = GetTargetEulerAngles(_deltaX);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(targetEulerAngles), _smoothSpeed);
        }

        public override void SetActive(bool value)
        {
            base.SetActive(value);

            Stop();
        }

        public void ResetRotation(bool instant, Action completed = null)
        {
            _deltaX = _startAngleY;
            Vector3 targetEulerAngles = GetTargetEulerAngles(_deltaX);
            
            if (instant)
            {
                transform.localRotation = Quaternion.Euler(targetEulerAngles);
                
                completed?.Invoke();
            }
            else
            {
                StartCoroutine(RotateCoroutine());
            }
            
            return;
            
            IEnumerator RotateCoroutine()
            {
                while (Vector3.Distance(transform.localEulerAngles, targetEulerAngles) > 0.1f)
                {
                    transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(targetEulerAngles), _smoothSpeed);
                    
                    yield return null;
                }
                
                completed?.Invoke();
            }
        }

        private void Stop()
        {
            InUse = false;
        }

        private Vector3 GetTargetEulerAngles(float angleY)
        {
            return new Vector3(transform.localEulerAngles.x, angleY, transform.localEulerAngles.z);
        }
    }
}