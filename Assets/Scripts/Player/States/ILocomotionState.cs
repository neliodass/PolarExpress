using Player.Interfaces;

namespace Player.States
{
    public interface ILocomotionState
    {
        void Enter(PlayerController player);
        void Update(PlayerController player);
        void Exit(PlayerController player);
        bool CanJump();
    }
}