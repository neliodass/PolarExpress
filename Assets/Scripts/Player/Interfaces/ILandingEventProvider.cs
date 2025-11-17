using System;

namespace Player.Interfaces
{
    public interface ILandingEventProvider
    {
        event Action OnLanded;
    }
}