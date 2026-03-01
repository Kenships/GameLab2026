using UnityEngine;
using AudioType = _Project.Scripts.Core.AudioPooling.Interface.AudioType;

namespace _Project.Scripts.Core.Modules.Base_Class
{
    [RequireComponent(typeof(Collider))]
    public abstract class PickupModuleBase : Module, IHoldable
    {
        [Header("Pickup Settings")]
        [SerializeField] protected Vector3 pickupOffset;
        
        [Header("Pickup Audio")]
        [SerializeField] protected AudioClip pickUpSound;
        [SerializeField] protected float pickUpSoundVolume = 0.25f;
        [SerializeField] protected AudioClip putDownSound;
        [SerializeField] protected float putDownSoundVolume = 1.5f;
        
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
            
            _audioPooler.New2DAudio(pickUpSound).OnChannel(AudioType.Sfx).SetVolume(pickUpSoundVolume).Play();

            _isPickedUp = true;
        }

        public void Drop()
        {
            _colliderCache ??= GetComponent<Collider>();
            _colliderCache.enabled = true;
            gameObject.layer = GridSystem.ObjectOnGridLayer.ToLayerIndex();
            transform.SetParent(null);
            
            _audioPooler.New2DAudio(putDownSound).OnChannel(AudioType.Sfx).SetVolume(putDownSoundVolume).Play();

            _isPickedUp = false;
        }
    }
}
