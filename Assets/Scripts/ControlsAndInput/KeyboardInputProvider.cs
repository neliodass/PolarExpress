using UnityEngine;

namespace ControlsAndInput
{
    public class KeyboardInputProvider : MonoBehaviour,IInputProvider
    {
        private PlayerControls _playerControls;
        private Vector2 _currentMovementInput;
        private bool _jumpPressedThisFrame;
        private bool _jumpHeld;
        private bool _crouchHeld;
        private bool _sprintHeld;
        private bool _interactHeld;

        private void Awake()
        {
            
            _playerControls = new PlayerControls();
            //Movement
            _playerControls.Player.Move.performed += ctx => _currentMovementInput = ctx.ReadValue<Vector2>();
            _playerControls.Player.Move.canceled += ctx => _currentMovementInput = Vector2.zero;
            //Jump
            _playerControls.Player.Jump.started += ctx => _jumpHeld = true;
            _playerControls.Player.Jump.performed += ctx => _jumpPressedThisFrame = true;
            _playerControls.Player.Jump.canceled += ctx => _jumpHeld = false;
            //Crouch
            _playerControls.Player.Crouch.started += ctx => _crouchHeld = true;
            _playerControls.Player.Crouch.canceled += ctx => _crouchHeld = false;
            //Sprint
            _playerControls.Player.Sprint.started += ctx => _sprintHeld = true;
            _playerControls.Player.Sprint.canceled += ctx => _sprintHeld = false;
            //Interact
            _playerControls.Player.Interact.started += ctx => _interactHeld = true;
            _playerControls.Player.Interact.canceled += ctx => _interactHeld = false;
            
            
            
        }

        private void OnEnable()
        {
            _playerControls.Enable();
        }
        private void OnDisable()
        {
            _playerControls.Disable();
        }

        private void LateUpdate()
        {
            _jumpPressedThisFrame = false;
        }
        public Vector3 GetMovementVector()
        {
            return new Vector3(_currentMovementInput.x, 0, _currentMovementInput.y);
        }
        public bool GetJumpButtonDown()
        {
            return _jumpPressedThisFrame;
        }

        public bool GetJumpButtonHeld()
        {
            return _jumpHeld;
        }
        public bool GetSprintButtonHeld()
        {
            return _sprintHeld;
        }

        public bool GetCrouchButtonHeld()
        {
            return _crouchHeld;
        }
        public bool GetInteractButtonHeld()
        {
            return _interactHeld;
        }
    }
}