using _Project.Scripts.Core.Enemies.Factories;
using UnityEngine;

namespace _Project.Scripts.Core.Enemies
{
    public class EnemySpawner : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private EnemyFactoryBase[] enemyFactories;
        [SerializeField] private Transform[] spawnPositions;
        [SerializeField] private Transform vhsLocation; //enemies move toward this

        [Header("Spawn Settings")]
        [SerializeField] private float spawnDelayMin = 1f;
        [SerializeField] private float spawnDelayMax = 3f; //spawns are random from these two values

        [Header("Enemy Settings")]
        [SerializeField] private float enemyMoveSpeed = 5f;
        [SerializeField] private int enemyHealth = 100;
        [SerializeField] private int enemyDamage = 10;
        [SerializeField] private float enemyAttackCooldown = 1f;

        private float _spawnTimer;

        private void Start()
        {
            SetRandomSpawnTimer();
        }

        private void Update()
        {
            _spawnTimer -= Time.deltaTime;

            if (_spawnTimer <= 0f)
            {
                SpawnEnemy();
                SetRandomSpawnTimer();
            }
        }

        private void SpawnEnemy()
        {
            int randomIndex = Random.Range(0, spawnPositions.Length);
            Transform spawnPoint = spawnPositions[randomIndex];
            
            int randEnemyIndex = Random.Range(0, enemyFactories.Length);
            
            EnemyBase enemy = enemyFactories[randEnemyIndex].CreateEnemy();
            enemy.transform.position = spawnPoint.position;
        }

        private void SetRandomSpawnTimer()
        {
            _spawnTimer = Random.Range(spawnDelayMin, spawnDelayMax);
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
}
