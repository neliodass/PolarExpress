using System;
using UnityEngine;

namespace Player.Interfaces
{
    public interface IFootstepEventProvider
    {
        event Action OnStep;
    }
}