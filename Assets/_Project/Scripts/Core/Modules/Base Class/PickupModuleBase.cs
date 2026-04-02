using _Project.Scripts.Core.Grid;
using _Project.Scripts.Core.Modules.Interface;
using _Project.Scripts.Util;
using UnityEngine;
using AudioType = _Project.Scripts.Core.AudioPooling.Interface.AudioType;

namespace _Project.Scripts.Core.Modules.Base_Class
{
    [RequireComponent(typeof(Collider))]
    public abstract class PickupModuleBase : HpModuleBase, IHoldable
    {
        [SerializeField] protected bool enableOnStart;
        [Header("Pickup Settings")]
        [SerializeField] protected Vector3 pickupOffset;
        [SerializeField] protected Vector3 pickupRotationOffset;

        [Header("Pickup Audio")]
        [SerializeField] protected AudioClip pickUpSound;
        [SerializeField] protected float pickUpSoundVolume = 0.25f;
        [SerializeField] protected AudioClip putDownSound;
        [SerializeField] protected float putDownSoundVolume = 1.5f;
        
        [Header("Object Light")]
        [SerializeField] protected GameObject groundLight;

        private Collider _colliderCache;
        public bool IsPickedUp {get; private set;}

        protected override void Start()
        {
            base.Start();
            EnableModule = enableOnStart;
            groundLight.SetActive(true);
            state = ModuleState.Used;
        }

        public void Anchor(Transform anchorTransform)
        {
            transform.SetParent(anchorTransform);
            transform.localPosition = pickupOffset;
            transform.localRotation = Quaternion.Euler(pickupRotationOffset);
        }

        public void PickUp()
        {
            _colliderCache ??= GetComponent<Collider>();
            _colliderCache.isTrigger = true;
            gameObject.layer = LayerMask.NameToLayer("HeldObject");
            
            groundLight.SetActive(false);
            
            _audioPooler.New2DAudio(pickUpSound).OnChannel(AudioType.Sfx).SetVolume(pickUpSoundVolume).Play();
            IsPickedUp = true;
        }

        public void Drop()
        {
            EnableModule = true;
            
            _colliderCache ??= GetComponent<Collider>();
            _colliderCache.isTrigger = false;
            gameObject.layer = GridSystem.ObjectOnGridLayer.ToLayerIndex();
            transform.SetParent(null);
            
            groundLight.SetActive(true);
            
            _audioPooler.New2DAudio(putDownSound).OnChannel(AudioType.Sfx).SetVolume(putDownSoundVolume).Play();
            IsPickedUp = false;
        }

        public void RotateClockWise()
        {
            transform.Rotate(Vector3.up, 90f);
        }
    }
}
