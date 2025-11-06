using UnityEngine;

namespace ControlsAndInput
{
    public interface IInputProvider
    {
        Vector3 GetMovementVector();
        bool GetJumpButtonDown();
        bool GetJumpButtonHeld();
        bool GetSprintButtonHeld();
        bool GetCrouchButtonHeld();
        bool GetInteractButtonHeld();
    }
}