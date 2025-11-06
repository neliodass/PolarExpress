using UnityEngine;

namespace Player.Interfaces
{
    public interface ICameraFollower
    {
        void SetTarget(Transform target);
        void FollowTarget();
        void ClearTarget();
        void SetOffset(Vector3 offset);
        
        Vector3 Offset { get; }
        
    }
}