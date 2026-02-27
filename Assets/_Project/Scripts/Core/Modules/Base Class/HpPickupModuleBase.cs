using _Project.Scripts.Core.HealthManagement;
using _Project.Scripts.Util.ExtensionMethods;
using UnityEngine;

namespace _Project.Scripts.Core.Modules.Base_Class
{
    public abstract class HpPickupModuleBase : PickupModuleBase
    {
        [Header("Time Settings")]
    
        [SerializeField] protected float maxHealth = 100f;
        [SerializeField] protected float defaultRecoverySpeed = 10f;
        [SerializeField] protected float rewindRecoveryMultiplier = 4f;
        [SerializeField] protected float defaultDecaySpeed = 5f;
        [SerializeField] protected float attackStateDecayMultiplier = 4f;
        
        private Health _health;
        protected bool _isRewinding;
        
        private void OnEnable()
        {
            _health = gameObject.GetOrAdd<Health>();
            _health.Initialize(maxHealth);
            _health.OnDeath += OnDeath;
            _health.OnFullHp += OnFullHp;
        }
        
        private void OnDeath()
        {
            state = ModuleState.Used;
        }

        private void OnFullHp()
        {
            state = ModuleState.Load;
        }
        
        public override void Rewind()
        {
            transform.localScale = 1.05f * Vector3.one;
            _isRewinding = state != ModuleState.Attack;
        }
        
        protected override void LoadState()
        {
            if (_isRewinding)
            {
                _health.AddToHealth(defaultRecoverySpeed * rewindRecoveryMultiplier * Time.deltaTime);
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
                _health.AddToHealth(defaultRecoverySpeed * rewindRecoveryMultiplier * Time.deltaTime);
            }
            else
            {
                _health.AddToHealth(defaultRecoverySpeed * Time.deltaTime);
            }
        }

        public override void CancelRewind()
        {
            transform.localScale = Vector3.one;
            _isRewinding = false;
        }
    
        public override void FastForward()
        {
            if (_isRewinding || state == ModuleState.Used)
                return;
        
            transform.localScale = 1.05f * Vector3.one;
        
            state = ModuleState.Attack;
        }

        public override void CancelFastForward()
        {
            if (state == ModuleState.Used)
                return;
        
            transform.localScale = Vector3.one;
        
            state = ModuleState.Load;
        }
    }
}
