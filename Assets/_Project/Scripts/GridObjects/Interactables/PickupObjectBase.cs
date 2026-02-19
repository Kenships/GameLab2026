using _Project.Scripts.Interaction.Interface;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

[RequireComponent(typeof(Collider))]
public abstract class PickupObjectBase : MonoBehaviour, IHoldable
{
    [Header("Pickup Settings")]
    [SerializeField] protected Vector3 pickupOffset;
    private Collider _colliderCache;

    public void Anchor(Transform anchorTransform)
    {
        transform.SetParent(anchorTransform);
        transform.localPosition = pickupOffset;
    }
    
    public void PickUp()
    {
        _colliderCache ??= GetComponent<Collider>();
        _colliderCache.enabled = false;
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
    }

    public void Drop()
    {
        _colliderCache ??= GetComponent<Collider>();
        _colliderCache.enabled = true;
        gameObject.layer = GridSystem.ObjectOnGridLayer.ToLayerIndex();
        transform.SetParent(null);
    }
}
