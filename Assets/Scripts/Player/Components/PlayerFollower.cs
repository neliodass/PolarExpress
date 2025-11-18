using Player.Interfaces;
using UnityEngine;

namespace Player.Components
{
    public class PlayerFollower : MonoBehaviour, ICameraFollower
    {
        [Header("Follow Settings")] [SerializeField]
        private Transform _target; //i.e .player transform

        [SerializeField] private Vector3 _offset = new Vector3(0f, 0.5f, 0.5f); //default offset
        [SerializeField] private float _followSpeed = 10f; //
        [SerializeField] private bool _followTarget = true;
        [SerializeField] private float _smoothTime = 0.15f;
        private Vector3 _currentVelocity;
        
        private Vector3 _targetOffset;
        private Vector3 _currentOffset;
        private Vector3 _offsetVelocity;

        private void Awake()
        {
            _currentOffset = _offset;
            _targetOffset = _offset;
        }
        
        public void SetTarget(Transform target)
        {
            _target = target;
        }
        public void FollowTarget()
        {
            _followTarget = true;
        }
        public void ClearTarget()
        {
            _followTarget = false;
            _target = null;
        }
        public void SetOffset(Vector3 offset)
        {
            _targetOffset = offset;
        }
        public Vector3 Offset => _offset;
        
        private void LateUpdate()
        {
          
            Debug.DrawRay(transform.position, transform.forward * 2f, Color.red);
            if (_target && _followTarget)
            {
                _currentOffset = Vector3.SmoothDamp(
                    _currentOffset, 
                    _targetOffset, 
                    ref _offsetVelocity, 
                    _smoothTime
                );
                Vector3 desiredPosition = _target.position + _target.rotation * _currentOffset;
                transform.position = desiredPosition;
                
                Quaternion targetYawRotation = Quaternion.Euler(0, _target.eulerAngles.y, 0);            
                transform.rotation = Quaternion.Slerp(transform.rotation, targetYawRotation, Time.deltaTime * _followSpeed);           
            }
        }
        
    }
}