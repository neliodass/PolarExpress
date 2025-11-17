using Player.Interfaces;

namespace Player.States.VerticalStates
{
    public interface IVerticalState
    {
        void Enter(PlayerController player);
        void Update(PlayerController player);
        void Exit(PlayerController player);
    }
}