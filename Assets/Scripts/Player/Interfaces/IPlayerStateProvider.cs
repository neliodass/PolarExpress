namespace Player.Interfaces
{
    public interface IPlayerStateProvider
    {

        float GetCurrentSpeed();
        float GetBaseSpeed();
        bool IsGrounded();
    }
}