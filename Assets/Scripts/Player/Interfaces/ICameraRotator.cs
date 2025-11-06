using UnityEngine;
namespace Player.Interfaces
{
    public interface ICameraRotator
    {
        void RotateVertical(float verticalRotationAmount);
        float GetCurrentVerticalAngle();
    }
}