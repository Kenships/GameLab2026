using System.Collections;
using UnityEngine;

namespace _Project.Scripts.Core.Modules
{
    public class RecordDiscShooter : PickupObjectBase
    {
        [Header("References")]
        [SerializeField] RecordDiscBullet recordDiscPrefab;
        [SerializeField] private Transform spawnPoint;

        [Header("Shooting Settings")]
        [SerializeField] private float rotateSpeed = 200f; // homing rotation sharpness (higher the sharper)
        [SerializeField] private float shootSpeed = 10f;
        [SerializeField] private float shootSpeed_fast = 15f;
        [SerializeField] private float shootSpeed_slow = 2.5f;
        [SerializeField] private float timeBetweenShots = 1f; //yk
        [SerializeField] [Range(0f, 1f)] private float bulletWobble = 0f; // o = bullet shoots straght (like frisbee) | 1= bullet wobbles (me throwing a frisbee)
        [SerializeField] private int maxTargets = 3; // max number of hits until bullet is destroyed
        [SerializeField] private float detectionRange = 15f; // range around shooter that detects enemies
        [SerializeField] private LayerMask enemyLayer;
        [SerializeField] protected float attackStateDuration = 15f;

        private float _shootTimer;
        private Transform _currentTarget;
        private float currentShootSpeed;
        private Coroutine attackCoroutine;

        private void Start()
        {
            currentShootSpeed = shootSpeed;
        }
        private void Update()
        {
            FindClosestEnemy();

            if (!_currentTarget) return;

            _shootTimer -= Time.deltaTime;

            if (_shootTimer <= 0f)
            {
                Shoot();
                _shootTimer = timeBetweenShots;
            }
        }

        private void FindClosestEnemy()
        {
            Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, detectionRange, enemyLayer);

            float closestDistance = Mathf.Infinity;
            Transform closestEnemy = null;

            foreach (Collider enemy in enemiesInRange)
            {
                float distance = Vector3.Distance(transform.position, enemy.transform.position);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = enemy.transform;
                }
            }

            _currentTarget = closestEnemy;
        }

        private void Shoot()
        {
            if (!_currentTarget) return;

            Vector3 directionToEnemy = (_currentTarget.position - spawnPoint.position).normalized;
            Quaternion rotationToEnemy = Quaternion.LookRotation(directionToEnemy);
        

            RecordDiscBullet bullet = Instantiate(recordDiscPrefab, spawnPoint.position, rotationToEnemy);
            bullet.Initialize(_currentTarget, currentShootSpeed, maxTargets, rotateSpeed, bulletWobble, enemyLayer);
        }

        protected override void LoadState()
        {
            if (attackCoroutine != null) StopCoroutine(attackCoroutine);
            currentShootSpeed = shootSpeed;
        }

        protected override void AttackState()
        {
            currentShootSpeed = shootSpeed_fast;
            attackCoroutine = StartCoroutine(AttackStateCoroutine());
        }

        protected override void UsedState()
        {
            currentShootSpeed = shootSpeed_slow;
            attackCoroutine = null;
        }

        protected override void OnStateChanged(ModuleState newState)
        {
            throw new System.NotImplementedException();
        }

        public override void Rewind()
        {
            throw new System.NotImplementedException();
        }

        public override void CancelRewind()
        {
            throw new System.NotImplementedException();
        }

        public override void CancelFastForward()
        {
            throw new System.NotImplementedException();
        }

        public override void FastForward()
        {
            throw new System.NotImplementedException();
        }

        private IEnumerator AttackStateCoroutine()
        {
            yield return new WaitForSeconds(attackStateDuration);
            state = ModuleState.Used;
            ActByState();
        }
    }
}
    
