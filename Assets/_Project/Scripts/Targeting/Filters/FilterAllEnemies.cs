using System.Collections.Generic;
using _Project.Scripts.Core.Enemies;
using _Project.Scripts.Targeting.Interface;

namespace _Project.Scripts.Targeting.Filters
{
    [System.Serializable]
    public struct FilterAllEnemies : ITargetingFilter<EnemyBase>
    {
        public List<EnemyBase> Filter(List<EnemyBase> targets)
        {
            return targets;
        }
    }
}
