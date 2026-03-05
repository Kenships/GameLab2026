using System;
using System.Collections.Generic;
using _Project.Scripts.Core.Enemies;
using _Project.Scripts.Util.CustomAttributes;
using UnityEngine;

namespace _Project.Scripts.Targeting
{
    [Serializable]
    public struct FilterByHp : ITargetingFilter<EnemyBase>
    {
        public enum FilterMode
        {
            Highest,
            Lowest,
            Above,
            Below
        }
        [SerializeField] private FilterMode mode;
        
        private bool IsHighestOrLowest => mode is FilterMode.Highest or FilterMode.Lowest;
        private bool IsAboveOrBelow => mode is FilterMode.Above or FilterMode.Below;
        
        [SerializeField, ShowIf(nameof(IsHighestOrLowest))] private int amount;
        [SerializeField, ShowIf(nameof(IsAboveOrBelow))] private float value;
        
        public List<EnemyBase> Filter(List<EnemyBase> targets)
        {
            switch (mode)
            {
                case FilterMode.Highest:
                    bool GreaterThan(EnemyBase a, EnemyBase b) => a.Health > b.Health;
                    return FilterExtrema(targets, GreaterThan);
                case FilterMode.Lowest:
                    bool LessThan(EnemyBase a, EnemyBase b) => a.Health < b.Health;
                    return FilterExtrema(targets, LessThan);
                case FilterMode.Above:
                    bool GreaterThanThreshold(EnemyBase a, float threshold) => a.Health > threshold;
                    return FilterThreshold(targets, GreaterThanThreshold);
                case FilterMode.Below:
                    bool LessThanThreshold(EnemyBase a, float threshold) => a.Health < threshold;
                    return FilterThreshold(targets, LessThanThreshold);
            }
            
            return new List<EnemyBase>();
        }

        public List<EnemyBase> FilterExtrema(List<EnemyBase> targets, Func<EnemyBase, EnemyBase, bool> compare)
        {
            return SortingUtil.GetFirstN(amount, targets, compare);
        }

        public List<EnemyBase> FilterThreshold(List<EnemyBase> targets, Func<EnemyBase, float, bool> compare) 
        {
            List<EnemyBase> filtered = new List<EnemyBase>();
            foreach (EnemyBase target in targets)
            {
                if (compare(target, value))
                {
                    filtered.Add(target);
                }
            }
            return filtered;
        }
    }
}
