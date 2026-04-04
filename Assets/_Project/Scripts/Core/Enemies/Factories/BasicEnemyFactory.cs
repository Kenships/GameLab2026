using _Project.Scripts.Core.Enemies.Interface;
using UnityEngine;
using UnityEngine.AI;

namespace _Project.Scripts.Core.Enemies.Factories
{
    [CreateAssetMenu(fileName = "BasicEnemyFactory", menuName = "Enemy Factories/BasicEnemyFactory")]
    public class BasicEnemyFactory : EnemyFactoryBase
    {
        [SerializeField] private GameObject enemyPrefab;
        [Header("Configs")]
        [SerializeField] private float maxHealth;

        [SerializeField] private float moveSpeed;
        [SerializeField] private float timeBetweenAttacks;
        [SerializeField] private float attackDamage;
        
        public override EnemyBase CreateEnemy(Vector3 position, Quaternion rotation)
        {
            GameObject enemy = Instantiate(enemyPrefab);
            NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
            agent.Warp(position);
            BasicEnemy basicEnemy = enemy.AddComponent<BasicEnemy>();
            basicEnemy.Initialize(maxHealth, moveSpeed, timeBetweenAttacks, attackDamage);
            Collider collider = enemy.GetComponent<Collider>();
            collider.enabled = true;
            return basicEnemy;
        }
    }
}
