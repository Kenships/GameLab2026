using System;
using _Project.Scripts.Core.HealthManagement;
using UnityEngine;
using UnityEngine.AI;

namespace _Project.Scripts.Core.Enemies
{
    public class BasicEnemy : MonoBehaviour, IDamageable
    {
        private Transform _target; //takes from EnemySpawnManager
        private Health _health;
        private float _attackCooldown; //takes from EnemySpawnManager
        private int _attackDamage; //takes from  EnemySpawnManager

        private bool _atVhs; //true if arrived at VHS

        private float _damageTimer;
        
        private NavMeshAgent _navMeshAgent;

        public void Initialize(Transform target, Health health, float moveSpeed, float attackCooldown, int attackDamage)
        {
            _atVhs = false;
            _target = target;
            _health = health;
            _attackCooldown = attackCooldown;
            _attackDamage = attackDamage;
            
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _navMeshAgent.speed = moveSpeed;
            
            _health.OnDeath += OnDeath;
        }

        private void OnDestroy()
        {
            _health.OnDeath -= OnDeath;
        }

        private void Update()
        {
            if (!_target) return;
            
            
            
            float distance = Vector3.Distance(transform.position, _target.position);

            _damageTimer -= Time.deltaTime;
            
            if (distance > 2f)
            {
                // Move towards the VHS location
                _navMeshAgent.SetDestination(_target.position);
            }
            else
            {
                
                //Debug.Log("Arrived at VHS");
                if (_damageTimer <= 0f)
                {
                    DamageVhs(_attackDamage);
                    _damageTimer = _attackCooldown;
                }
            }
        }

        private void DamageVhs(int damage)
        {
            if (_target.TryGetComponent(out IDamageable damageable))
            {
                damageable.Damage(damage);
            }
        }

        public void Damage(float damage)
        {
            _health.AddToHealth(-damage);
        }
        
        private void OnDeath()
        {
            Destroy(gameObject);
        }
    }
}
