using UnityEngine;

namespace ControlsAndInput
{
    public class MouseInputProvider : MonoBehaviour, IMouseInput
    {
        private PlayerControls _playerControls;
        private Vector2 _currentLookDelta;

        private void Awake()
        {
            _playerControls = new PlayerControls();
            _playerControls.Player.Look.performed += ctx => _currentLookDelta = ctx.ReadValue<Vector2>();
            _playerControls.Player.Look.canceled += ctx => _currentLookDelta = Vector2.zero;
        }
        private void OnEnable()
        {
            _playerControls.Enable();
        }

        private void OnDisable()
        {
            _playerControls.Disable();
        }
        public Vector2 GetLookDelta()
        {
            return _currentLookDelta;
        }
        
    }
}