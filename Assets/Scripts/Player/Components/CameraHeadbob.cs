using System;
using Player.Interfaces;
using UnityEngine;

namespace Player.Components
{
    public class CameraHeadbob : MonoBehaviour
    {
        [Header("State Provider")]
        [Tooltip("Drag the Player object here (the one with PlayerController)")]
        [SerializeField]
        private MonoBehaviour _stateProviderSource;

        private IPlayerStateProvider _stateProvider;

        [Header("Headbob Settings")] [Tooltip("How fast the bob should be at base speed (WalkSpeed)")] [SerializeField]
        private float _baseBobSpeed = 5f;

        [Tooltip("How strong (height) the bob should be at base speed (WalkSpeed)")] [SerializeField]
        private float _baseBobAmount = 0.1f;

        [Tooltip("How quickly the bob returns to the start position (smaller = smoother)")] [SerializeField]
        private float _smoothTime = 0.1f;

        [Tooltip("Bob speed multiplier - how much the player's speed accelerates the animation")] [SerializeField]
        private float _speedMultiplier = 1.0f;

        [Tooltip("Bob amount multiplier - how much the player's speed increases the bob height")] [SerializeField]
        private float _amountMultiplier = 1.0f;


        private Vector3 _startLocalPosition;
        private float _timer = 0f;
        private Vector3 _bobVelocity = Vector3.zero;

        private const float STEP_CYCLE_THRESHOLD = Mathf.PI;
        private float _lastStepTime = 0f;

        private void Awake()
        {
            _stateProvider = _stateProviderSource.GetComponent<IPlayerStateProvider>();
            if (_stateProvider == null)
            {
                Debug.LogError("IPlayerStateProvider not found on _stateProviderSource!", this);
                enabled = false;
                return;
            }

            _startLocalPosition = transform.localPosition;
        }

        private void LateUpdate()
        {
            bool isGrounded = _stateProvider.IsGrounded();
            float currentSpeed = _stateProvider.GetCurrentSpeed();
            float baseSpeed = _stateProvider.GetBaseSpeed();
            float speedRatio = (baseSpeed > 0.01f) ? (currentSpeed / baseSpeed) : 0f;

            if (!isGrounded || speedRatio < 0.1f)
            {
                _timer = 0f;
                transform.localPosition = Vector3.SmoothDamp(
                    transform.localPosition,
                    _startLocalPosition,
                    ref _bobVelocity,
                    _smoothTime
                );
            }
            else
            {
                float targetBobSpeed = _baseBobSpeed * Mathf.Clamp(speedRatio, 0.5f, 2f) * _speedMultiplier;
                float targetBobAmount = _baseBobAmount * Mathf.Clamp(speedRatio, 0.5f, 2f) * _amountMultiplier;
                _timer += targetBobSpeed * Time.unscaledDeltaTime;

                float bobOffset = Mathf.Abs(Mathf.Sin(_timer)) * targetBobAmount;
                Vector3 targetPosition = _startLocalPosition + new Vector3(0, bobOffset, 0);
                transform.localPosition = Vector3.SmoothDamp(
                    transform.localPosition,
                    targetPosition,
                    ref _bobVelocity,
                    _smoothTime / 2f
                );
            }
        }
    }
}