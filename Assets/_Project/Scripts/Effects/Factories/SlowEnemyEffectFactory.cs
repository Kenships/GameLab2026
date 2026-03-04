using System;
using _Project.Scripts.Core.Enemies;
using _Project.Scripts.Effects.Interface;
using _Project.Scripts.Effects.Runtime;
using UnityEngine;

namespace _Project.Scripts.Effects.Factories
{
    [Serializable]
    public class SlowEnemyEffectFactory : IEffectFactory<EnemyBase>
    {
        [SerializeField] private float duration;
        [SerializeField] private float slowFactor;
        
        public IEffect<EnemyBase> CreateEffect()
        {
            return new SlowEnemyEffect
                   {
                       Duration = duration, 
                       SlowFactor = slowFactor
                   };
        }
    }
}
