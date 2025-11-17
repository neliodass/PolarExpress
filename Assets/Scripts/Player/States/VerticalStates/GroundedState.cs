using Player.Interfaces;

namespace Player.States.VerticalStates
{
    public class GroundedState:IVerticalState
    {
        public void Enter(PlayerController player)
        {
            player.Movable.ResetVerticalVelocity();
            player.ReportLanding();
        }

        public void Update(PlayerController player)
        {
            if (player.InputProvider.GetJumpButtonDown())
            {
                var canJump = player.GetCurrentLocomotionState().CanJump();
                if (canJump)
                {
                    player.Movable.Jump(player.Movable.JumpForce);
                    player.TransitionToVerticalState(player.AirborneState);
                    return;
                }
            }

            if (!player.Movable.IsGrounded)
            {
                player.TransitionToVerticalState(player.AirborneState);
            }
        }

        public void Exit(PlayerController player)
        {
            
        }
    }
}