using Player.Interfaces;
using UnityEngine;
namespace Player.Components

{
    [RequireComponent(typeof(Camera))]
    public class CameraViewHandler:MonoBehaviour, ICameraView
    {
        [Header("State Provider")]
        [SerializeField] private MonoBehaviour _stateProviderSource;
        private IPlayerStateProvider _stateProvider;
        
        [Header("Sprint FOV Settings")]
        [SerializeField] private  readonly float _sprintFovMultiplier = 1.2f;
        [SerializeField] private readonly float _fovSmoothTime = 0.2f;
        private bool _isManagedBySprint = true;
        private float _fovVelocity;
        private Camera _camera;
        private float _defaultFov;

        private void Awake()
        {
            _camera = GetComponent<Camera>();
            _defaultFov = _camera.fieldOfView;
            _stateProvider = _stateProviderSource.GetComponent<IPlayerStateProvider>();
            if (_stateProvider == null)
            {
                Debug.LogError("Player State Provider is null", this);
                enabled = false;
            }
        }

        private void LateUpdate()
        {
            if (!_isManagedBySprint)
            {
                _fovVelocity = 0f;
                return;
            }
            bool isSprinting = 
                               (_stateProvider.GetCurrentSpeed() > _stateProvider.GetBaseSpeed() + 0.1f);
            float targetFov = isSprinting ? _defaultFov*_sprintFovMultiplier : _defaultFov;
            _camera.fieldOfView = Mathf.SmoothDamp(_camera.fieldOfView, targetFov, ref _fovVelocity, _fovSmoothTime);
            
        }

        public void SetFieldOfView(float fov)
        {
            _defaultFov = fov;
            _camera.fieldOfView = fov;
        }

        public void ResetFieldOfView()
        {
            _camera.fieldOfView = _defaultFov;
            
        }
    }
}