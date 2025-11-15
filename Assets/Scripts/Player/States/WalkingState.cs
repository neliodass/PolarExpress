using Player.Interfaces;
using UnityEngine;

namespace Player.States
{
    public class WalkingState:ILocomotionState
    {
        public void Enter(PlayerController player)
        {
            
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
            Vector3 movementInput = player.InputProvider.GetMovementVector();
            if (player.InputProvider.GetCrouchButtonHeld())
            {
                player.TransitionToLocomotionState(player.CrouchingState);
                return;
            }
            if(player.InputProvider.GetSprintButtonHeld() && movementInput.z>0 &&player.GetCurrentVerticalState() == player.GroundedState)
            {
                player.TransitionToLocomotionState(player.SprintingState);
                return;
            }

            if (movementInput.magnitude < 0.05f)
            {
                player.TransitionToLocomotionState(player.IdleState);
                return;
            }
            player.Movable.Move(movementInput,player.WalkSpeed);
        }
    }
}