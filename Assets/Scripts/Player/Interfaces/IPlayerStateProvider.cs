using Player.States.LocomotionStates;
using Player.States.VerticalStates;

namespace Player.Interfaces
{
    public interface IPlayerStateProvider
    {

        float GetCurrentSpeed();
        float GetBaseSpeed();
        bool IsGrounded();
        bool IsSprinting();
        bool IsCrouching();
        IVerticalState GetCurrentVerticalState();
        ILocomotionState GetCurrentLocomotionState();
    }
}