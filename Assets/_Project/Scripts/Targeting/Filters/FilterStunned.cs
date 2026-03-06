using System;
using System.Collections.Generic;
using _Project.Scripts.Core.Enemies;
using _Project.Scripts.Targeting.Interface;
using UnityEngine;

namespace _Project.Scripts.Targeting.Filters
{
    [Serializable]
    public struct FilterStunned : ITargetingFilter<EnemyBase>
    {
        [SerializeField] private bool invert;
        
        public List<EnemyBase> Filter(List<EnemyBase> targets)
        {
            List<EnemyBase> filteredTargets = new List<EnemyBase>();
            foreach (EnemyBase target in targets)
            {
                if (target.Stunned == !invert)
                {
                    filteredTargets.Add(target);
                }
            }
            return filteredTargets;
        }
    }
}
