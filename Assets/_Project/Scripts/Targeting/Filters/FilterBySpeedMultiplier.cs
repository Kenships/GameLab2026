using _Project.Scripts.Core.Enemies;

namespace _Project.Scripts.Targeting.Filters
{
    [System.Serializable]
    public class FilterBySpeedMultiplier : FilterByExtrema<EnemyBase>
    {
        protected override bool GreaterThan(EnemyBase a, EnemyBase b)
        {
            return a.SpeedMultiplier > b.SpeedMultiplier;
        }

        protected override bool LessThan(EnemyBase a, EnemyBase b)
        {
            return a.SpeedMultiplier < b.SpeedMultiplier;
        }

        protected override bool GreaterThanThreshold(EnemyBase a, float threshold)
        {
            return a.SpeedMultiplier > threshold;
        }

        protected override bool LessThanThreshold(EnemyBase a, float threshold)
        {
            return a.SpeedMultiplier < threshold;
        }
    }
}
