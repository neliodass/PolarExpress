using Player.Interfaces;
using UnityEngine;

namespace Player.States
{
    public class IdleState:ILocomotionState
    {
        public void Enter(PlayerController player)
        {
            player.Movable.Move(Vector3.zero,0);
        }

        public void Exit(PlayerController player)
        {
            
        }

        public bool CanJump()
        {
            return true;
        }
        public void Update(PlayerController player)
        {
            if (player.InputProvider.GetCrouchButtonHeld())
            {
                player.TransitionToLocomotionState(player.CrouchingState);
                return;
            }
            Vector3 movementInput = player.InputProvider.GetMovementVector();
            if (movementInput.magnitude > 0.05f)
            {
                if (player.InputProvider.GetJumpButtonHeld()
                    && player.GetCurrentVerticalState() == player.GroundedState)
                {
                    player.TransitionToLocomotionState(player.SprintingState);
                }
                else
                {
                    player.TransitionToLocomotionState(player.WalkingState);
                }
                return;
            }
            player.Movable.Move(Vector3.zero,0);
        }
        
    }
}