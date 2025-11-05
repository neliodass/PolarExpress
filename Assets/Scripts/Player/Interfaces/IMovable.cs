using UnityEngine;

namespace Player.Interfaces
{
    public interface IMovable
    {
        void Move(Vector3 direction,float speed);
        void Jump(float force);
        void Rotate(float horizontalRotationAmount);
        bool IsGrounded { get; }
        float JumpForce { get; set; }
    }
}