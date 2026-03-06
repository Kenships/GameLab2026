using System;
using System.Collections.Generic;
using _Project.Scripts.Core.Enemies;
using _Project.Scripts.Targeting.Interface;
using UnityEngine;

namespace _Project.Scripts.Targeting.Strategies
{
    [Serializable]
    public class EnemyTargetingStrategy : ITargetingStrategy<EnemyBase>
    {
        
        [SerializeReference, SubclassSelector] private EnemyTargetingStrategy fallbackStrategy;
        [SerializeReference, SubclassSelector] private ITargetingFilter<EnemyBase>[] filters;
        
        public List<EnemyBase> Evaluate(List<EnemyBase> targets)
        {
            if (targets.Count == 0)
            {
                return targets;
            }

            List<EnemyBase> filteredTargets = new(targets);
            
            foreach (ITargetingFilter<EnemyBase> targetingStrategy in filters)
            {
                filteredTargets = targetingStrategy.Filter(filteredTargets);
            }
            
            if (filteredTargets.Count > 0)
            {
                return filteredTargets;
            }
            
            return fallbackStrategy != null ? fallbackStrategy.Evaluate(targets) : targets;
        }
    }
}
