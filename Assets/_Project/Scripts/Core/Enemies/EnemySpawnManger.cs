using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform[] spawnPositions;
    [SerializeField] private Transform vhsLocation; //enemies move toward this

    [Header("Spawn Settings")]
    [SerializeField] private float spawnDelayMin = 1f;
    [SerializeField] private float spawnDelayMax = 3f; //spawwns are random from these two values

    [Header("Enemy Settings")]
    [SerializeField] private float enemyMoveSpeed = 5f;
    [SerializeField] private int enemyHealth = 100;
    [SerializeField] private int enemyDamage = 10;
    [SerializeField] private float enemyAttackCooldown = 1f;

    private float spawnTimer;

    private void Start()
    {
        SetRandomSpawnTimer();
    }

    private void Update()
    {
        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0f)
        {
            SpawnEnemy();
            SetRandomSpawnTimer();
        }
    }

    private void SpawnEnemy()
    {
        int randomIndex = Random.Range(0, spawnPositions.Length);
        Transform spawnPoint = spawnPositions[randomIndex];
        
        GameObject enemy = Instantiate(
            enemyPrefab,
            spawnPoint.position,
            spawnPoint.rotation
        );
        
        EnemyMovement movement = enemy.AddComponent<EnemyMovement>();
        movement.Initialize(vhsLocation, enemyMoveSpeed, enemyAttackCooldown, enemyDamage);

        Health health = enemy.AddComponent<Health>();
        health.Initialize(enemyHealth);
    }

    private void SetRandomSpawnTimer()
    {
        spawnTimer = Random.Range(spawnDelayMin, spawnDelayMax);
    }

    private void OnDrawGizmosSelected()
    {
        if (spawnPositions != null)
        {
            Gizmos.color = Color.green;
            foreach (Transform spawnPoint in spawnPositions)
            {
                if (spawnPoint != null)
                {
                    Gizmos.DrawWireSphere(spawnPoint.position, 0.5f);
                }
            }
        }
        
        if (vhsLocation != null && spawnPositions != null)
        {
            Gizmos.color = Color.yellow;
            foreach (Transform spawnPoint in spawnPositions)
            {
                if (spawnPoint != null)
                {
                    Gizmos.DrawLine(spawnPoint.position, vhsLocation.position);
                }
            }
        }
    }
}