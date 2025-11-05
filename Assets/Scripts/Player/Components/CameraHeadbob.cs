using Player.Interfaces;
using UnityEngine;

namespace Player.Components
{
    public class CameraHeadbob:MonoBehaviour
    {
        [Header("State Provider")]
        [Tooltip("Przeciągnij tu obiekt Gracza (ten z PlayerController)")]
        [SerializeField] private MonoBehaviour _stateProviderSource;
        private IPlayerStateProvider _stateProvider;

        [Header("Headbob Settings")]
        [Tooltip("Jak szybki ma być 'bob' przy bazowej prędkości (WalkSpeed)")]
        [SerializeField] private float _baseBobSpeed = 10f;

        [Tooltip("Jak mocny (wysoki) ma być 'bob' przy bazowej prędkości (WalkSpeed)")]
        [SerializeField] private float _baseBobAmount = 0.05f;

        [Tooltip("Jak szybko 'bob' ma wracać do pozycji startowej (mniejsza = płynniej)")]
        [SerializeField] private float _smoothTime = 0.1f;
        
        [Tooltip("Mnożnik prędkości 'bobu' - jak bardzo prędkość gracza przyspiesza animację")]
        [SerializeField] private float _speedMultiplier = 1.0f;
        
        [Tooltip("Mnożnik mocy 'bobu' - jak bardzo prędkość gracza zwiększa wysokość animacji")]
        [SerializeField] private float _amountMultiplier = 1.0f;

        
        
        private Vector3 _startLocalPosition;
        private float _timer = 0f;
        private Vector3 _bobVelocity = Vector3.zero;

        private void Awake()
        {
            _stateProvider = _stateProviderSource.GetComponent<IPlayerStateProvider>();
            if (_stateProvider == null)
            {
                Debug.LogError("Nie znaleziono IPlayerStateProvider na _stateProviderSource!", this);
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