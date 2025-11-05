namespace Player.Interfaces
{
    public interface ICrouchable 
    {
        void SetCrouch(bool isCrouching);
        bool IsCrouching { get; }
        bool CanStandUp();
    }

}