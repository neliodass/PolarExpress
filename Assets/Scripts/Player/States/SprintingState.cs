using Player.Interfaces;
using UnityEngine;

namespace Player.States
{
    public class SprintingState : ILocomotionState
    {
        public void Enter(PlayerController player)
        {
        }

        public void Exit(PlayerController player)
        {
        }

        public bool CanJump() => true;

        public void Update(PlayerController player)
        {
            Vector3 movementInput = player.InputProvider.GetMovementVector();
            if (player.InputProvider.GetCrouchButtonHeld())
            {
                //TODO: SlidingState
                player.TransitionToLocomotionState(player.CrouchingState);
            }

            if (!player.InputProvider.GetSprintButtonHeld() ||
                movementInput.z < 0 )
            {
                player.TransitionToLocomotionState(player.WalkingState);
                return;
            }

            if (movementInput.magnitude < 0.05f)
            {
                player.TransitionToLocomotionState(player.IdleState);
                return;
            }

            player.Movable.Move(movementInput, player.SprintSpeed);
        }
    }
}