using System;
using _Project.Scripts.Core.Enemies;

namespace _Project.Scripts.Targeting.Filters
{
    [Serializable]
    public class FilterEnemiesByHp : FilterByExtrema<EnemyBase>
    {
        protected override bool GreaterThan(EnemyBase a, EnemyBase b)
        {
            return a.Health > b.Health;
        }

        protected override bool LessThan(EnemyBase a, EnemyBase b)
        {
            return a.Health < b.Health;
        }

        protected override bool GreaterThanThreshold(EnemyBase a, float threshold)
        {
            return a.Health > threshold;
        }

        protected override bool LessThanThreshold(EnemyBase a, float threshold)
        {
            return a.Health < threshold;
        }
    }
}
