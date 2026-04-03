using _Project.Scripts.Core.HealthManagement;
using _Project.Scripts.Util.ExtensionMethods;
using UnityEngine;

namespace _Project.Scripts.Core.Modules.Base_Class
{
    public abstract class HpModuleBase : Module
    {
        [Header("Time Settings")]
        [SerializeField] protected float maxHealth = 100f;
        [SerializeField] protected float defaultRecoverySpeed = 40f;
        [SerializeField] protected float defaultDecaySpeed = 5f;
        [SerializeField] protected float attackStateDecayMultiplier = 4f;
        
        protected Health _health;
        
        protected override void OnAwake()
        {
            _health = gameObject.GetOrAdd<Health>();
            _health.Initialize(maxHealth, 0f);
        }

        protected virtual void Start()
        {
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
        }

        private void OnFullHp()
        {
            state = ModuleState.Load;
        }
        
        protected override void LoadState()
        {
            float multiplier = 1f;
            if (_isRewinding)
            {
                if (IsDoubleRewind()) multiplier = 2f;
                _health.AddToHealth(defaultRecoverySpeed * multiplier * Time.deltaTime);
            }
            else
            {
                if (IsDoubleFastForward()) multiplier = 2f;
                _health.AddToHealth(-defaultDecaySpeed * multiplier * Time.deltaTime);
            }
        }
        
        protected override void AttackState()
        {
            float multiplier = IsDoubleFastForward() ? 2f : 1f;
            _health.AddToHealth(-defaultDecaySpeed * attackStateDecayMultiplier * multiplier * Time.deltaTime);
        }
        
        protected override void UsedState()
        {
            if (_isRewinding)
            {
                float multiplier = IsDoubleRewind() ? 2f : 1f;
                _health.AddToHealth(defaultRecoverySpeed * multiplier * Time.deltaTime);
            }

            if (_currentFastForwardSound != null)
            {
                _currentFastForwardSound.Stop();
                _currentFastForwardSound = null;
            }
        }
    }
}
