using Player.Interfaces;
using UnityEngine;

namespace Player.States
{
    public class CrouchingState:ILocomotionState
    {
        public void Enter(PlayerController player)
        {
            player.Crouchable.SetCrouch(true);
            
            player.CameraFollower.SetOffset(player.crouchCameraOffset);
        }
        public void Exit(PlayerController player)
        {
            player.Crouchable.SetCrouch(false);
            player.CameraFollower.SetOffset(player.originalCameraOffset);
        }
        public bool CanJump()
        {
            return false;
        }

        public void Update(PlayerController player)
        {
            Vector3 movementInput = player.InputProvider.GetMovementVector();

            if (!player.InputProvider.GetCrouchButtonHeld())
            {
                if (player.Crouchable.CanStandUp())
                {
                    player.TransitionToLocomotionState(player.IdleState);
                    return;
                }
                player.Movable.Move(movementInput, player.CrouchSpeed);
            }
        }
    }
}