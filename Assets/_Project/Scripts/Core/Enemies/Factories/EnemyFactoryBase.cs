using _Project.Scripts.Core.Enemies.Interface;
using UnityEngine;

namespace _Project.Scripts.Core.Enemies.Factories
{
    public abstract class EnemyFactoryBase : ScriptableObject, IEnemyFactory
    {
        public abstract EnemyBase CreateEnemy(Vector3 position, Quaternion rotation);
    }
}
