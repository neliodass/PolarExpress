using Player.Interfaces;

namespace Player.States.VerticalStates
{
    public class AirborneState:IVerticalState
    {
        public void Enter(PlayerController player)
        {
            
        }
        public void Update(PlayerController player)
        {
            player.Movable.ApplyGravity();
            if (player.Movable.IsGrounded)
            {
                player.TransitionToVerticalState(player.GroundedState);
            }
        }
        public void Exit(PlayerController player)
        {
            
        }
        
    }
}