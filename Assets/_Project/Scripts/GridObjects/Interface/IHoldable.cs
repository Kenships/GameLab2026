using UnityEngine;

namespace _Project.Scripts.Interaction.Interface
{
    public interface IHoldable 
    {
        void Anchor(Transform transform);
        void PickUp();
        void Drop();
    }
}
