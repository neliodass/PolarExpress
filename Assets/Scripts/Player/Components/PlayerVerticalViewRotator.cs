using Player.Interfaces;
using UnityEngine;

namespace Player.Components
{
    public class PlayerVerticalViewRotator : MonoBehaviour,ICameraRotator, IInvertible
    {
        [Header("Rotation Settings")] [SerializeField]
        private readonly float _verticalRotationSpeed = 1f;

        [SerializeField] private float _minPitch = -80f;
        [SerializeField] private readonly float _maxPitch = 80;
        [SerializeField] private bool _isInverted = false;

        private float _currentPitch = 0f;
        public float GetCurrentVerticalAngle()=> _currentPitch;
        public bool IsInverted => _isInverted;
        private float _inversionFactor = 1f;
        
        public void SetInverted(bool isInverted)
        {
            _isInverted = isInverted;
            UpdateInversionFactor();
        }
        
        private void OnValidate()
        {
            UpdateInversionFactor();
            if (_minPitch > _maxPitch)
            {
                _minPitch = _maxPitch;
            }
        }

        private void UpdateInversionFactor()
        {
            _inversionFactor = _isInverted ? -1f : 1f;
        }

        public void RotateVertical(float deltaY)
        {
            _currentPitch -= deltaY * _inversionFactor * _verticalRotationSpeed*Time.deltaTime;
            _currentPitch = Mathf.Clamp(_currentPitch, _minPitch, _maxPitch);
        }

        private void LateUpdate()
        {
            transform.localRotation = Quaternion.Euler(_currentPitch, 0f, 0f);
        }
    }
}