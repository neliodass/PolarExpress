using System;
using Player.Interfaces;
using UnityEngine;

namespace Player.Components
{
    public class ProceduralFootstepProvider:MonoBehaviour,IFootstepEventProvider
    {
        [Header("State Provider")]
        [Tooltip("Drag the Player object here (the one with PlayerController)")]
        [SerializeField] private MonoBehaviour _stateProviderSource;
        private IPlayerStateProvider _stateProvider;

        [Header("Headbob Settings")]
        [Tooltip("How fast the bob should be at base speed (WalkSpeed)")]
        [SerializeField] private float _baseBobSpeed = 5f;
        [Tooltip("Bob speed multiplier - how much the player's speed accelerates the animation")]
        [SerializeField] private float _speedMultiplier = 1.0f;
      
        
        public event Action OnStep;
        private float _timer = 0f;
        private const float StepCycleThreshold = Mathf.PI;
        private float _lastStepTime = 0f;


        private void Awake()
        {
            _stateProvider = _stateProviderSource.GetComponent<IPlayerStateProvider>();
            if (_stateProvider == null)
            {
                Debug.LogError("Player state provider was not found.");
                enabled = false;
                return;
            }
            
            
        }

        private void Update()
        {
            bool isGrounded = _stateProvider.IsGrounded();
            float currentSpeed = _stateProvider.GetCurrentSpeed();
            float baseSpeed = _stateProvider.GetBaseSpeed();
            float speedRatio = (baseSpeed > 0.01f) ? (currentSpeed / baseSpeed) : 0f;
            if(!isGrounded || speedRatio < 0.1f)
            {
                _timer = 0f;
                _lastStepTime = 0f;
            }

            else
            {
                float targetBobSpeed = _baseBobSpeed * Mathf.Clamp(speedRatio, 0.5f, 2f) * _speedMultiplier;
                _timer += targetBobSpeed * Time.deltaTime;

                if (_timer - _lastStepTime >= StepCycleThreshold)
                {
                    OnStep?.Invoke();
                    _lastStepTime = _timer;
                }
            }
        }
    }
}