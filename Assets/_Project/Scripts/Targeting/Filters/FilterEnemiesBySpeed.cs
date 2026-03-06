using System;
using System.Collections.Generic;
using _Project.Scripts.Core.Enemies;
using _Project.Scripts.Targeting.Interface;
using _Project.Scripts.Util.CustomAttributes;
using UnityEngine;

namespace _Project.Scripts.Targeting.Filters
{
    [Serializable]
    public class FilterEnemiesBySpeed : FilterByExtrema<EnemyBase>
    {
        protected override bool GreaterThan(EnemyBase a, EnemyBase b)
        {
            return a.Speed > b.Speed;
        }

        protected override bool LessThan(EnemyBase a, EnemyBase b)
        {
            return a.Speed < b.Speed;
        }

        protected override bool GreaterThanThreshold(EnemyBase a, float threshold)
        {
            return a.Speed > threshold;
        }

        protected override bool LessThanThreshold(EnemyBase a, float threshold)
        {
            return a.Speed < threshold;
        }
    }
}
