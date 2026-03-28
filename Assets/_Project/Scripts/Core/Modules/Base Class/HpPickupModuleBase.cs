using _Project.Scripts.Core.AudioPooling.Interface;
using _Project.Scripts.Core.HealthManagement;
using _Project.Scripts.Util.ExtensionMethods;
using UnityEngine;
using AudioType = _Project.Scripts.Core.AudioPooling.Interface.AudioType;

namespace _Project.Scripts.Core.Modules.Base_Class
{
    public abstract class HpPickupModuleBase : PickupModuleBase
    {
        [Header("Time Settings")]
        [SerializeField] protected float maxHealth = 100f;
        [SerializeField] protected float defaultRecoverySpeed = 40f;
        [SerializeField] protected float defaultDecaySpeed = 5f;
        [SerializeField] protected float attackStateDecayMultiplier = 4f;

        private IAudioPlayer currentFastForwardSound;
        private IAudioPlayer currentRewindSound;
        protected bool _isRewinding;
        protected bool _isFastForwarding;
        protected Health _health;
        // LazerCannon
        protected bool isTriggerTypeModule = false;
        
        private void OnEnable()
        {
            _health = gameObject.GetOrAdd<Health>();
            _health.Initialize(maxHealth, 0f);
        }

        protected override void Start()
        {
            base.Start();
            _health.OnDeath += OnDeath;
            _health.OnFullHp += OnFullHp;
        }
        
        protected virtual void OnDestroy()
        {
            _health.OnDeath -= OnDeath;
            _health.OnFullHp -= OnFullHp;
        }
        
        
        private void OnDeath()
        {
            state = ModuleState.Used;

            _isFastForwarding = false;
            if (currentFastForwardSound != null)
            {
                currentFastForwardSound.Stop();
                currentFastForwardSound = null;
            }
        }

        private void OnFullHp()
        {
            state = ModuleState.Load;
        }

        private void LateUpdate()
        {
            if(!isTriggerTypeModule && state == ModuleState.Used && _health.CurrentHealth > 0)
            {
                state = ModuleState.Load;
            }
        }

        public override void Rewind()
        {
            if (_isFastForwarding || _isRewinding)
            {
                return;
            }
            
            _isRewinding = state != ModuleState.Attack;

            currentRewindSound = _audioPooler.New2DAudio(rewindSound).OnChannel(AudioType.Sfx)
                .SetVolume(rewindSoundVolume).LoopAudio().Play();
        }
        
        protected override void LoadState()
        {
            if (_isRewinding)
            {
                _health.AddToHealth(defaultRecoverySpeed * Time.deltaTime);
            }
            else
            {
                _health.AddToHealth(- defaultDecaySpeed * Time.deltaTime);
            }
        }
        
        protected override void AttackState()
        {
            _health.AddToHealth(-defaultDecaySpeed * attackStateDecayMultiplier * Time.deltaTime);
        }
        
        protected override void UsedState()
        {
            if (_isRewinding)
            {
                _health.AddToHealth(defaultRecoverySpeed * Time.deltaTime);
            }

            if (currentFastForwardSound != null)
            {
                currentFastForwardSound.Stop();
                currentFastForwardSound = null;
            }
        }

        public override void CancelRewind()
        {
            _isRewinding = false;
            currentRewindSound?.Stop();
            currentRewindSound = null;
        }
    
        public override void FastForward()
        {
            if (_isRewinding || _isFastForwarding)
                return;

            _isFastForwarding = true;
            state = ModuleState.Attack;

            currentFastForwardSound = _audioPooler.New2DAudio(fastForwardSound).OnChannel(AudioType.Sfx)
                .SetVolume(fastForwardSoundVolume).LoopAudio().Play();
        }

        public override void CancelFastForward()
        {
            if (state == ModuleState.Used)
                return;
        
            state = ModuleState.Load;

            _isFastForwarding = false;
            currentFastForwardSound?.Stop();
            currentFastForwardSound = null;
        }
    }
}
