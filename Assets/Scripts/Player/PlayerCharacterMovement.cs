using Player.Interfaces;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerCharacterMovement : MonoBehaviour, IMovable, ICrouchable
    {
        [Header("Movement")] [SerializeField] private float _gravity = -9.81f;
        [SerializeField] private float _jumpForce = 3f;
        [SerializeField] private float _groundedCheckOffset = -0.1f;
        [SerializeField] private float _groundedCheckRadius = 0.3f;
        [SerializeField] private LayerMask _groundMask;

        [Header("Crouch Settings")] [SerializeField]
        private float _crouchHeight = 1.0f;
        [SerializeField] private LayerMask _standUpBlockMask;
        private CharacterController _characterController;
        private Vector3 _currentMovementDirection;
        private float _currentMovementSpeed;
        private Vector3 _verticalVelocity;
        private bool _isGrounded;
        
        private float _actualCurrentSpeedMagnitude;

        private float _originalHeight;
        private Vector3 _originalCenter;
        private Vector3 _crouchCenter;
        public bool IsCrouching { get; private set; }


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
                return;
            }

            _originalHeight = _characterController.height;
            _originalCenter = _characterController.center;
            float oldBase = _originalCenter.y - (_originalHeight / 2f);
            float newBase = oldBase + (_crouchHeight / 2f);
            _crouchCenter = new Vector3(_originalCenter.x, newBase, _originalCenter.z);
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
            if (IsCrouching) return;
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
            _actualCurrentSpeedMagnitude =
                new Vector3(_characterController.velocity.x, 0, _characterController.velocity.z).magnitude;
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

        public void SetCrouch(bool isCrouching)
        {
            if (isCrouching == IsCrouching) return;
            IsCrouching = isCrouching;
            if (IsCrouching)
            {
                _characterController.height = _crouchHeight;
                _characterController.center = _crouchCenter;
            }
            else
            {
                _characterController.height = _originalHeight;
                _characterController.center = _originalCenter;
            }
        }

        public bool CanStandUp()
        {
            Vector3 capsuleCenter = transform.position + _originalCenter;
            float halfHeight = (_originalHeight / 2f) - _characterController.radius;
            Vector3 point1 = capsuleCenter + (Vector3.down * halfHeight);
            Vector3 point2 = capsuleCenter + (Vector3.up * halfHeight);
            float radius = _characterController.radius;
            bool isBlocked = Physics.CheckCapsule(point1, point2, radius, _standUpBlockMask,QueryTriggerInteraction.Ignore);
            return !isBlocked;
        }
        private void OnDrawGizmosSelected()
        {
            // Ta funkcja narysuje w edytorze kapsułę, którą sprawdzamy
            // Będzie widoczna tylko, gdy zaznaczysz gracza w scenie
        
            // Zabezpieczenie, jeśli Awake() jeszcze nie ruszyło
            if (_characterController == null || _originalHeight == 0)
            {
                // Spróbuj pobrać wartości na szybko, jeśli jesteśmy w edytorze
                CharacterController controller = GetComponent<CharacterController>();
                if (controller == null) return;
            
                _originalHeight = controller.height;
                _originalCenter = controller.center;
            }

            // --- Dokładnie ta sama logika co w CanStandUp() ---
            Vector3 capsuleCenter = transform.position + _originalCenter;
            float halfHeight = (_originalHeight / 2f) - _characterController.radius;
            Vector3 point1 = capsuleCenter + (Vector3.down * halfHeight);
            Vector3 point2 = capsuleCenter + (Vector3.up * halfHeight);
            float radius = _characterController.radius;
        
            bool isBlocked = Physics.CheckCapsule(
                point1, point2, radius, _standUpBlockMask, QueryTriggerInteraction.Ignore
            );
            // --- Koniec logiki ---

            // Narysuj kapsułę: czerwoną jeśli zablokowana, zieloną jeśli wolna
            Gizmos.color = isBlocked ? Color.red : Color.green;
        
            Gizmos.DrawWireSphere(point1, radius);
            Gizmos.DrawWireSphere(point2, radius);
        
            // Narysuj 4 linie łączące, żeby wyglądało jak kapsuła
            Gizmos.DrawLine(point1 + Vector3.right * radius, point2 + Vector3.right * radius);
            Gizmos.DrawLine(point1 + Vector3.left * radius, point2 + Vector3.left * radius);
            Gizmos.DrawLine(point1 + Vector3.forward * radius, point2 + Vector3.forward * radius);
            Gizmos.DrawLine(point1 + Vector3.back * radius, point2 + Vector3.back * radius);
        }
    }
}