using _Project.Scripts.Interaction.Interface;
using UnityEngine;

namespace _Project.Scripts.GridObjects.Interactables
{
    [RequireComponent(typeof(Collider))]
    public abstract class PickupObjectBase : MonoBehaviour, IHoldable
    {
        [Header("Pickup Settings")]
        [SerializeField] protected Vector3 pickupOffset;
        private Collider _colliderCache;
        protected bool _isPickedUp;

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
        
            _isPickedUp = true;
        }

        public void Drop()
        {
            _colliderCache ??= GetComponent<Collider>();
            _colliderCache.enabled = true;
            gameObject.layer = GridSystem.ObjectOnGridLayer.ToLayerIndex();
            transform.SetParent(null);
        
            _isPickedUp = false;
        }
    }
}
