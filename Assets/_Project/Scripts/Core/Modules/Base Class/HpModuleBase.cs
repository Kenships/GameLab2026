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

            if (_currentFastForwardSound != null)
            {
                _currentFastForwardSound.Stop();
                _currentFastForwardSound = null;
            }
        }
    }
}
