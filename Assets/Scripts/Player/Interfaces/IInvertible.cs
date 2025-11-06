using UnityEngine;

namespace Player.Interfaces
{
    public interface IInvertible
    {
        void SetInverted(bool inverted);
        bool IsInverted { get; }
    }
}