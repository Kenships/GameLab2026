using _Project.Scripts.GridObjects.Interactables;
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
        [SerializeField] private float shootSpeed = 10f; // speed of bullet
        [SerializeField] private float timeBetweenShots = 1f; //yk
        [SerializeField] [Range(0f, 1f)] private float bulletWobble = 0f; // o = bullet shoots straght (like frisbee) | 1= bullet wobbles (me throwing a frisbee)
        [SerializeField] private int maxTargets = 3; // max number of hits until bullet is destroyed
        [SerializeField] private float detectionRange = 15f; // range around shooter that detects enemies
        [SerializeField] private LayerMask enemyLayer;

        private float _shootTimer;
        private Transform _currentTarget;

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
            bullet.Initialize(_currentTarget, shootSpeed, maxTargets, rotateSpeed, bulletWobble, enemyLayer);
        }
    }
}
    
