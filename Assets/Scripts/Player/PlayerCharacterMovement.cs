using Player.Interfaces;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerCharacterMovement : MonoBehaviour, IMovable
    {
        [Header("Movement")] [SerializeField] private float _gravity = -9.81f;
        [SerializeField] private float _jumpForce = 3f;
        [SerializeField] private float _groundedCheckOffset = -0.1f;
        [SerializeField] private float _groundedCheckRadius = 0.3f;
        [SerializeField] private LayerMask _groundMask;

        private CharacterController _characterController;
        private Vector3 _currentMovementDirection;
        private float _currentMovementSpeed;
        private Vector3 _verticalVelocity;
        private bool _isGrounded;
        
        private float _actualCurrentSpeedMagnitude; 

        public bool IsGrounded => _isGrounded;

        public float JumpForce
        {
            get => _jumpForce;
            set => _jumpForce = value;
        }

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            if (_characterController == null)
            {
                Debug.LogError("CharacterController not found");
                enabled = false;
            }
        }

        private void Update()
        {
            CheckGroundStatus();
            ApplyGravity();
            ApplyMovement();
            Debug.DrawRay(transform.position, transform.forward * 2f, Color.red);
        }

        public void Move(Vector3 direction, float speed)
        {
            _currentMovementDirection = transform.TransformDirection(direction);
            _currentMovementDirection.y = 0;
            _currentMovementDirection.Normalize();
            _currentMovementDirection = Vector3.ClampMagnitude(_currentMovementDirection, 1f);
            _currentMovementSpeed = speed;
        }

        public void Jump(float force)
        {
            _verticalVelocity.y = Mathf.Sqrt(force * -2f * _gravity);
        }

        public void Rotate(float horizontalRotationAmount)
        {
            transform.Rotate(Vector3.up, horizontalRotationAmount);
        }

        private void ApplyGravity()
        {
            if (_isGrounded)
            {
                if (_verticalVelocity.y < 0f)
                    _verticalVelocity.y = -2f;
            }
            else
            {
                _verticalVelocity.y += _gravity * Time.deltaTime;
            }
        }

        private void ApplyMovement()
        {
            Vector3 finalMovement = (_currentMovementDirection * _currentMovementSpeed) +
                                    (_verticalVelocity.y * Vector3.up);
            _characterController.Move(finalMovement * Time.deltaTime);
            _actualCurrentSpeedMagnitude = new Vector3(_characterController.velocity.x, 0, _characterController.velocity.z).magnitude;
        }

        private void CheckGroundStatus()
        {
            Vector3 sphereOrigin = transform.position +
                                   Vector3.down * ((_characterController.height / 2f) - _groundedCheckOffset);
            _isGrounded = Physics.CheckSphere(sphereOrigin, _groundedCheckRadius, _groundMask);
        }
        
        public float GetCurrentSpeed()
        {
            return _actualCurrentSpeedMagnitude;
        }
        public bool IsMoving()
        {
            return _actualCurrentSpeedMagnitude > 0.05f; 
        }
    }
}