using UnityEngine;

public class RecordDiscShooter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject recordDiscPrefab;
    [SerializeField] private Transform spawnPoint;

    [Header("Shooting Settings")]
    [SerializeField] private float rotateSpeed = 200f; // homing rotation sharpness (higher the sharper)
    [SerializeField] private float shootSpeed = 10f; // speed of bullet
    [SerializeField] private float timeBetweenShots = 1f; //yk
    [SerializeField] [Range(0f, 1f)] private float bulletWobble = 0f; // o = bullet shoots straght (like frisbee) | 1= bullet wobbles (me throwing a frisbee)
    [SerializeField] private int maxTargets = 3; // max number of hits until bullet is destroyed
    [SerializeField] private float detectionRange = 15f; // range around shooter that detects enemies
    [SerializeField] private LayerMask enemyLayer;

    private float shootTimer;
    private Transform currentTarget;

    private void Update()
    {
        FindClosestEnemy();

        if (currentTarget == null) return;

        shootTimer -= Time.deltaTime;

        if (shootTimer <= 0f)
        {
            Shoot();
            shootTimer = timeBetweenShots;
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

        currentTarget = closestEnemy;
    }

    private void Shoot()
    {
        if (currentTarget == null) return;

        Vector3 directionToEnemy = (currentTarget.position - spawnPoint.position).normalized;
        Quaternion rotationToEnemy = Quaternion.LookRotation(directionToEnemy);

        GameObject disc = Instantiate(
            recordDiscPrefab,
            spawnPoint.position,
            rotationToEnemy
        );

        RecordDiscBullet bullet = disc.GetComponent<RecordDiscBullet>();
        bullet.Initialize(currentTarget, shootSpeed, maxTargets, rotateSpeed, bulletWobble, enemyLayer);
    }
}
    
