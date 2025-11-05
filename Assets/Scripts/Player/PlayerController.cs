using ControlsAndInput;
using UnityEngine;

namespace Player.Interfaces
{
    [RequireComponent(typeof(PlayerCharacterMovement))]
    public class PlayerController : MonoBehaviour, IInvertible, IPlayerStateProvider
    {
        [Header("Dependecies")] [SerializeField]
        private MonoBehaviour _inputProviderSource;

        private IInputProvider _inputProvider;

        [SerializeField] private MonoBehaviour _movementSource;
        private IMovable _movable;

        [SerializeField] private MonoBehaviour _lookSource;
        private IMouseInput _mouseInput;
        [SerializeField] private MonoBehaviour _cameraRotatorSource;
        private ICameraRotator _cameraRotator;
        [SerializeField] private MonoBehaviour _cameraFollowerSource;
        private ICameraFollower _cameraFollower;
        
        [SerializeField] private MonoBehaviour _crouchSource;
        private ICrouchable _crouchable;

        [Header("Movement Settings")] public float WalkSpeed = 5f;
        public float SprintSpeed = 8f;
        public float CrouchSpeed = 2f;
        public float JumpForce = 8f;
        [Header("Look Settings")] public float MouseSensitivity = 1f;
        public float PlayerRotationSpeed = 100f;
        [SerializeField] private bool _isInverted = false;
        
        [Header("Camera Settings")]
        [SerializeField] private float _crouchCameraHeightOffset = -1f;
        
        private float _inversionFactor = 1f;
        public bool IsInverted => _isInverted;
        
        
        private Vector3 _originalCameraOffset;
        private Vector3 _crouchCameraOffset;
        private bool _isCrouching = false;
        private void Awake()
        {
            if (!AssignSources())
            {
                return;
            }

            UpdateInversionFactor();
            _cameraFollower.SetTarget(transform);
            
            _originalCameraOffset = _cameraFollower.Offset;
            _crouchCameraOffset = _originalCameraOffset + new Vector3(0f, _crouchCameraHeightOffset, 0f);
            
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        private void UpdateInversionFactor()
        {
            _inversionFactor = _isInverted ? -1f : 1f;
        }
        public void SetInverted(bool inverted)
        {
            _isInverted = inverted;
            UpdateInversionFactor();
        }
        private void OnDestroy()
        {
        }

        private bool AssignSources()
            {
                _inputProvider = _inputProviderSource as IInputProvider;
                if (_inputProvider == null)
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

                _movable = _movementSource as IMovable;
                if (_movable == null)
                {
                    Debug.LogError("IMovable not found");
                    enabled = false;
                    return false;
                }
                
                _crouchable = _crouchSource.GetComponent<ICrouchable>();
                if (_crouchable == null)
                {
                    Debug.LogError("ICrouchable not found");
                    enabled = false;
                    return false;
                }
                
                
                _cameraRotator = _cameraRotatorSource as ICameraRotator;
                if (_cameraRotator == null)
                {
                    Debug.LogError("ICameraRotator not found");
                    //enabled = false;
                    return false;
                }
                _cameraFollower = _cameraFollowerSource as ICameraFollower;
                if (_cameraFollower == null)
                {
                    Debug.LogError("ICameraFollower not found");
                    //enabled = false;
                    return false;
                }

                return true;
            }

        private void Update()
        {
            HandleMovement();
            HandleLook();
            HandleJump();
            HandleCrouch();
        }

        private void HandleMovement()
        {
            Vector3 movementVector = _inputProvider.GetMovementVector();
            float speed = _inputProvider.GetSprintButtonHeld() ? SprintSpeed : WalkSpeed;
            _movable.Move(movementVector, speed);
        }

        private void HandleLook()
        {
            Vector2 mouseDelta = _mouseInput.GetLookDelta() * MouseSensitivity;
            _movable.Rotate(mouseDelta.x*_inversionFactor * PlayerRotationSpeed * Time.deltaTime);
            _cameraRotator?.RotateVertical(mouseDelta.y);
        }

        private void HandleJump()
        {
            if (!_inputProvider.GetJumpButtonDown()) return;
            if (_movable.IsGrounded)
            {
                _movable.Jump(_movable.JumpForce);
            }
        }
        
        private void HandleCrouch()
        {
            bool wantsToCrouch = _inputProvider.GetCrouchButtonHeld();
            if (wantsToCrouch && !_isCrouching)
            {
                _isCrouching = true;
                _cameraFollower.SetOffset(_crouchCameraOffset);
                _crouchable.SetCrouch(true);
            }
            else if (!wantsToCrouch && _isCrouching)
            {
                _isCrouching = false;
                _cameraFollower.SetOffset(_originalCameraOffset);
                _crouchable.SetCrouch(false);
            }
            
        }
        #region IPlayerStateProvider Implementation
        public float GetCurrentSpeed()
        {
            return _movable.GetCurrentSpeed();
        }
        public float GetBaseSpeed()
        {
            return WalkSpeed;
        }
        public bool IsGrounded()
        {
            
            return _movable.IsGrounded;
        }
        public bool IsCrouching()
        {
            return _isCrouching;
        }
        #endregion
    }
}