using _Project.Scripts.Core.HealthManagement;
using _Project.Scripts.Util.ExtensionMethods;
using UnityEngine;
using UnityEngine.AI;

namespace _Project.Scripts.Core.Enemies
{
    public class BasicEnemy : EnemyBase
    {
        private static Transform _cachedVhsTransform;
        
        private Transform _target;
        private float _moveSpeed;
        private float _attackCooldown; 
        private float _attackDamage; 
        private float _damageTimer;
        private NavMeshAgent _navMeshAgent;

        public void Initialize(float maxHealth, float moveSpeed, float attackCooldown, float attackDamage)
        {
            _cachedVhsTransform ??= FindFirstObjectByType<VHSModule>().transform;
            _target = _cachedVhsTransform;
            
            _health ??= gameObject.GetOrAdd<Health>();
            _health.Initialize(maxHealth);
            
            _attackCooldown = attackCooldown;
            _attackDamage = attackDamage;
            
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _moveSpeed = moveSpeed;
            
            _health.OnDeath += OnDeath;
        }

        private void FixedUpdate()
        {
            if (Stunned){
                _navMeshAgent.isStopped = true;
                return;
            }
            
            _navMeshAgent.isStopped = false;
            _navMeshAgent.speed = _moveSpeed * SpeedMultiplier;
            
            Debug.Log($"BasicEnemy Act {_target?.name}");
            if (!_target) return;
            
            float distance = Vector3.Distance(transform.position, _target.position);

            _damageTimer -= Time.deltaTime * SpeedMultiplier;
            
            if (distance > 2f)
            {
                // Move towards the VHS location
                _navMeshAgent.SetDestination(_target.position);
            }
            else
            {
                
                Debug.Log("Arrived at VHS");
                if (_damageTimer <= 0f)
                {
                    DamageVhs(_attackDamage);
                    _damageTimer = _attackCooldown;
                }
            }
        }

        private void DamageVhs(float damage)
        {
            if (_target.TryGetComponent(out IDamageable damageable))
            {
                damageable.Damage(damage);
            }
        }
    }
}
