using _Project.Scripts.Core.HealthManagement;
using _Project.Scripts.Core.Modules;
using _Project.Scripts.Util.ExtensionMethods;
using UnityEngine;
using UnityEngine.AI;

namespace _Project.Scripts.Core.Enemies
{
    public class BasicEnemy : EnemyBase
    {
        private static Transform _cachedVhsTransform;

        private Transform _target;
        private float _attackDamage;
        private NavMeshAgent _navMeshAgent;

        public void Initialize(float maxHealth, float moveSpeed, float attackCooldown, float attackDamage)
        {          
            _cachedVhsTransform = VHSModule.Location;
            _target = _cachedVhsTransform;

            _health ??= gameObject.GetOrAdd<Health>();
            _health.Initialize(maxHealth);

            _attackDamage = attackDamage;

            _navMeshAgent = GetComponent<NavMeshAgent>();
            _moveSpeed = moveSpeed;

            _health.OnDeath += OnDeath;
        }

        private void FixedUpdate()
        {
            if (Stunned)
            {
                _navMeshAgent.isStopped = true;
                return;
            }

            _navMeshAgent.isStopped = false;
            _navMeshAgent.speed = _moveSpeed * SpeedMultiplier;

            if (!_target) return;

            float distance = Vector3.Distance(transform.position, _target.position);

            if (distance > 1.4f)
            {
                _navMeshAgent.SetDestination(_target.position);
            }
            else
            {
                DamageVhs(_attackDamage);
            }
        }

        private void DamageVhs(float damage)
        {
            if (_target.TryGetComponent(out IDamageable damageable))
            {
                damageable.Damage(damage);

                // Play animation or effect here

                // Enemy dies after it attacks the vhs once
                Destroy(gameObject);
            }
        }
    }
}
