using System;
using System.ComponentModel;
using _Project.Scripts.Core.Grid;
using _Project.Scripts.Core.Modules.Interface;
using _Project.Scripts.Util;
using UnityEngine;
using AudioType = _Project.Scripts.Core.AudioPooling.Interface.AudioType;

namespace _Project.Scripts.Core.Modules.Base_Class
{
    [RequireComponent(typeof(Collider))]
    public abstract class PickupModuleBase : Module, IHoldable
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

        private Collider _colliderCache;
        protected bool _isPickedUp;

        protected virtual void Start()
        {
            EnableModule = enableOnStart;
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
            EnableModule = true;
            
            _colliderCache ??= GetComponent<Collider>();
            _colliderCache.isTrigger = true;
            gameObject.layer = LayerMask.NameToLayer("HeldObject");
            
            _audioPooler.New2DAudio(pickUpSound).OnChannel(AudioType.Sfx).SetVolume(pickUpSoundVolume).Play();
            _isPickedUp = true;
        }

        public void Drop()
        {
            _colliderCache ??= GetComponent<Collider>();
            _colliderCache.isTrigger = false;
            gameObject.layer = GridSystem.ObjectOnGridLayer.ToLayerIndex();
            transform.SetParent(null);
            
            _audioPooler.New2DAudio(putDownSound).OnChannel(AudioType.Sfx).SetVolume(putDownSoundVolume).Play();
            _isPickedUp = false;
        }

        public void RotateClockWise()
        {
            transform.Rotate(Vector3.up, 90f);
        }
    }
}
