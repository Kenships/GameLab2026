using UnityEngine;

namespace _Project.Scripts.Core.Modules.Interface
{
    public interface IHoldable
    {
        void Anchor(Transform transform);
        void PickUp();
        void Drop();
    }
}
