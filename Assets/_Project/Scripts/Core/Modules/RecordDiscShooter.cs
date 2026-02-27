using _Project.Scripts.Core.AudioPooling;
using System.Collections;
using UnityEngine;
using AudioType = _Project.Scripts.Core.AudioPooling.Interface.AudioType;

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

        [Header("Audio")]
        [SerializeField] private AudioClip shootingSound;

        private float _shootTimer;
        private Transform _currentTarget;
        private float currentShootSpeed;
        private float currentTimeBetweenShots;
        private Coroutine attackCoroutine;

        private void Start()
        {
            currentShootSpeed = shootSpeed;
            currentTimeBetweenShots = timeBetweenShots;
        }
        private void Update()
        {
            FindClosestEnemy();

            if (!_currentTarget) return;

            _shootTimer -= Time.deltaTime;

            if (_shootTimer <= 0f)
            {
                Shoot();
                _shootTimer = currentTimeBetweenShots;
                Debug.Log(currentTimeBetweenShots);
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

            _audioPooler.New2DAudio(shootingSound).OnChannel(AudioType.Sfx).Play();

            RecordDiscBullet bullet = Instantiate(recordDiscPrefab, spawnPoint.position, rotationToEnemy);
            bullet.Initialize(_currentTarget, currentShootSpeed, maxTargets, rotateSpeed, bulletWobble, enemyLayer);
        }

        protected override void LoadState()
        {
            if (attackCoroutine != null) StopCoroutine(attackCoroutine);
            currentShootSpeed = shootSpeed;
            currentTimeBetweenShots = timeBetweenShots;
        }

        protected override void AttackState()
        {
            currentShootSpeed = shootSpeed_fast;
            currentTimeBetweenShots = timeBetweenShots / (shootSpeed_fast / shootSpeed);
            attackCoroutine = StartCoroutine(AttackStateCoroutine());
        }

        protected override void UsedState()
        {
            currentShootSpeed = shootSpeed_slow;
            currentTimeBetweenShots = timeBetweenShots / (shootSpeed_slow / shootSpeed);
            attackCoroutine = null;
        }
        private IEnumerator AttackStateCoroutine()
        {
            yield return new WaitForSeconds(attackStateDuration);
            state = State.Used;
            ActByState();
        }
    }
}
    
