using Player.Interfaces;
using UnityEngine;

namespace Player.States.VerticalStates
{
    public class AirborneState:IVerticalState
    {
        private float timeEnteredState;
        private float landingGracePeriod = 0.1f; 
        public void Enter(PlayerController player)
        {
            timeEnteredState = Time.time;
        }
        public void Update(PlayerController player)
        {
            player.Movable.ApplyGravity();
            if (Time.time < timeEnteredState + landingGracePeriod)
            {
                return;
            }
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