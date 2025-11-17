using System;
using ControlsAndInput;
using Player.States;
using Player.States.LocomotionStates;
using Player.States.VerticalStates;
using UnityEngine;
using UnityEngine.Serialization;

namespace Player.Interfaces
{
    [RequireComponent(typeof(PlayerCharacterMovement))]
    public class PlayerController : MonoBehaviour, IInvertible, IPlayerStateProvider,ILandingEventProvider
    {
        [Header("Dependencies")] [SerializeField]
        private MonoBehaviour _inputProviderSource;

        [SerializeField] private MonoBehaviour _movementSource;

        [SerializeField] private MonoBehaviour _lookSource;
        [SerializeField] private MonoBehaviour _cameraRotatorSource;
        [SerializeField] private MonoBehaviour _cameraFollowerSource;

        [SerializeField] private MonoBehaviour _crouchSource;

        [Header("Movement Settings")] public float WalkSpeed = 5f;
        public float SprintSpeed = 8f;
        public float CrouchSpeed = 2f;
        public float jumpForce = 8f;
        [Header("Look Settings")] public float MouseSensitivity = 1f;
        public float PlayerRotationSpeed = 100f;
        [SerializeField] private bool _isInverted;

        [Header("Camera Settings")] private readonly float _crouchCameraHeightOffset = -1f;

        public ICameraFollower CameraFollower { get; private set; }
        private ICameraRotator _cameraRotator;
        public ICrouchable Crouchable { get; private set; }


        public event Action OnLanded;
        
        private float _inversionFactor = 1f;
        private bool _isCrouching;
        private IMouseInput _mouseInput;
        
        public Vector3 originalCameraOffset;
        public Vector3 crouchCameraOffset;

        public IInputProvider InputProvider { get; private set; }
        public IMovable Movable { get; private set; }
        private IVerticalState _currentVerticalState;
        public IVerticalState GetCurrentVerticalState() => _currentVerticalState;
        public IVerticalState GroundedState { get; private set; }
        public IVerticalState AirborneState { get; private set; }
        private ILocomotionState _currentLocomotionState;
        public ILocomotionState GetCurrentLocomotionState() => _currentLocomotionState;
        public ILocomotionState IdleState { get; private set; }
        public ILocomotionState WalkingState { get; private set; }
        public ILocomotionState SprintingState { get; private set; }
        public ILocomotionState CrouchingState { get; private set; }

        private void Awake()
        {
            if (!AssignSources()) return;

            GroundedState = new GroundedState();
            AirborneState = new AirborneState();
            
            IdleState = new IdleState();
            WalkingState = new WalkingState();
            SprintingState = new SprintingState();
            CrouchingState = new CrouchingState();

            UpdateInversionFactor();
            CameraFollower.SetTarget(transform);

            originalCameraOffset = CameraFollower.Offset;
            crouchCameraOffset = originalCameraOffset + new Vector3(0f, _crouchCameraHeightOffset, 0f);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Start()
        {
            _currentVerticalState = GroundedState;
            _currentVerticalState.Enter(this);
            _currentLocomotionState = IdleState;
            _currentLocomotionState.Enter(this);
        }

        private void Update()
        {
            Movable.CheckGroundStatus();
            _currentVerticalState.Update(this);
            _currentLocomotionState.Update(this);
            HandleLook();
        }

        public bool IsInverted => _isInverted;

        public void SetInverted(bool inverted)
        {
            _isInverted = inverted;
            UpdateInversionFactor();
        }

        private void UpdateInversionFactor()
        {
            _inversionFactor = _isInverted ? -1f : 1f;
        }


        private bool AssignSources()
        {
            InputProvider = _inputProviderSource as IInputProvider;
            if (InputProvider == null)
            {
                Debug.LogError("IInputProvider not found");
                enabled = false;
                return false;
            }

            _mouseInput = _lookSource as IMouseInput;
            if (_mouseInput == null)
            {
                Debug.LogError("IMouseInput not found");
                enabled = false;
                return false;
            }

            Movable = _movementSource.GetComponent<IMovable>();
            if (Movable == null)
            {
                Debug.LogError("IMovable not found");
                enabled = false;
                return false;
            }

            Crouchable = _crouchSource.GetComponent<ICrouchable>();
            if (Crouchable == null)
            {
                Debug.LogError("ICrouchable not found");
                enabled = false;
                return false;
            }


            _cameraRotator = _cameraRotatorSource as ICameraRotator;
            if (_cameraRotator == null)
            {
                Debug.LogError("ICameraRotator not found");
                return false;
            }

            CameraFollower = _cameraFollowerSource as ICameraFollower;
            if (CameraFollower == null)
            {
                Debug.LogError("ICameraFollower not found");
                return false;
            }

            return true;
        }

        public void TransitionToVerticalState(IVerticalState newState)
        {
            _currentVerticalState.Exit(this);
            _currentVerticalState = newState;
            _currentVerticalState.Enter(this);
        }
        public void TransitionToLocomotionState(ILocomotionState newState)
        {
            _currentLocomotionState.Exit(this);
            _currentLocomotionState = newState;
            _currentLocomotionState.Enter(this);
        }
        private void HandleLook()
        {
            var mouseDelta = _mouseInput.GetLookDelta() * MouseSensitivity;
            Movable.Rotate(mouseDelta.x * _inversionFactor * PlayerRotationSpeed * Time.deltaTime);
            _cameraRotator?.RotateVertical(mouseDelta.y);
        }

        public void ReportLanding()
        {
            OnLanded?.Invoke();
        }
        #region IPlayerStateProvider Implementation

        public float GetCurrentSpeed()
        {
            return Movable.GetCurrentSpeed();
        }

        public float GetBaseSpeed()
        {
            return WalkSpeed;
        }

        public bool IsGrounded()
        {
            return Movable.IsGrounded;
        }

        public bool IsCrouching()
        {
            return _currentLocomotionState == CrouchingState;
        }

        public bool IsSprinting()
        {
            return _currentLocomotionState == SprintingState;
        }

        #endregion
    }
}