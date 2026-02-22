using _Project.Scripts.Core.HealthManagement;
using UnityEngine;

namespace _Project.Scripts.Core.Enemies
{
    public class BasicEnemy : MonoBehaviour, IDamageable
    {
        private Transform _target; //takes from EnemySpawnManager
        private Health _health;
        private float _moveSpeed; //takes from  EnemySpawnManager
        private float _attackCooldown; //takes from EnemySpawnManager
        private int _attackDamage; //takes from  EnemySpawnManager

        private bool _atVhs; //true if arrived at VHS

        private float _damageTimer;

        public void Initialize(Transform target, Health health, float moveSpeed, float attackCooldown, int attackDamage)
        {
            _atVhs = false;
            _target = target;
            _health = health;
            _moveSpeed = moveSpeed;
            _attackCooldown = attackCooldown;
            _attackDamage = attackDamage;
            
            _health.OnDeath += OnDeath;
        }

        private void Update()
        {
            if (!_target) return;

            Vector3 direction = (_target.position - transform.position).normalized;
            // Face the target
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = targetRotation;

            float distance = Vector3.Distance(transform.position, _target.position);
            if (distance < 0.5f)
            {
                _atVhs = true;
            }

            _damageTimer -= Time.deltaTime;
            
            if (!_atVhs)
            {
                // Move towards the VHS location
                transform.position += direction * (_moveSpeed * Time.deltaTime);
            }
            else
            {
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
