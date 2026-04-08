using UnityEngine;

namespace _Project.Scripts.Core.Enemies.Interface
{
    public interface IEnemyFactory
    {
        EnemyBase CreateEnemy(Vector3 position, Quaternion rotation);
    }
}
