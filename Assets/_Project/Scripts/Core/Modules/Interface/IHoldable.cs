using UnityEngine;

public interface IHoldable
{
    void Anchor(Transform transform);
    void PickUp();
    void Drop();
}
