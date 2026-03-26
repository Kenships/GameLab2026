using System;
using UnityEngine;

namespace _Project.Scripts.Core.Modules.Interface
{
    public interface IHoldable
    {
        public event Action OnHold;
        public event Action OnRelease;
        public event Action OnRotateClockWise;
        
        void Anchor(Transform transform);
        void PickUp();
        void Drop();
        void RotateClockWise();
    }
}
