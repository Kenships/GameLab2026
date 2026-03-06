using _Project.Scripts.Core.Enemies.Interface;
using UnityEngine;

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
        
        public override EnemyBase CreateEnemy()
        {
            GameObject enemy = Instantiate(enemyPrefab);
            BasicEnemy basicEnemy = enemy.AddComponent<BasicEnemy>();
            basicEnemy.Initialize(maxHealth, moveSpeed, timeBetweenAttacks, attackDamage);
            return basicEnemy;
        }
    }
}
