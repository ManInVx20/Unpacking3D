using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace VinhLB
{
    public class IsometricCameraRotation : BaseController
    {
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

        private bool _canGetInput = true;
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
            
            if (_canGetInput && VinhLBUtility.IsPointerDown())
            {
                if (VinhLBUtility.IsPointerOnUI() 
                    || VinhLBUtility.TryCastRay(_camera, out _, float.PositiveInfinity, _blockLayer))
                {
                    _canGetInput = false;
                }
            }
            
            if (_canGetInput)
            {
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
            else
            {
                if (VinhLBUtility.IsPointerUp())
                {
                    _canGetInput = true;
                }
            }
        }
        
        private void LateUpdate()
        {
            Vector3 targetEulerAngles = GetTargetEulerAngles(_deltaX);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(targetEulerAngles), _smoothSpeed);
        }
        
        public override void SetActive(bool value)
        {
            base.SetActive(value);

            Stop();
        }
        
        public Tween ResetRotation(bool immediately, float duration = 1f, System.Action onComplete = null)
        {
            _deltaX = _startAngleY;
            Vector3 targetEulerAngles = GetTargetEulerAngles(_deltaX);
            
            if (immediately)
            {
                transform.localRotation = Quaternion.Euler(targetEulerAngles);
                
                onComplete?.Invoke();

                return null;
            }
            
            return transform.DOLocalRotate(targetEulerAngles, duration)
                .SetEase(Ease.InBack)
                .OnComplete(() => onComplete?.Invoke());
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