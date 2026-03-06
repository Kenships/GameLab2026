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
        [SerializeField] private SlowEnemyEffect.SlowType slowType;
        [SerializeField] private float duration;
        [Range(0, 100)]
        [SerializeField] private float slowPercentageFactor;
        
        public IEffect<EnemyBase> CreateEffect()
        {
            return new SlowEnemyEffect
                   {
                       Type = slowType,
                       Duration = duration, 
                       SlowPercentageFactor = slowPercentageFactor
                   };
        }
    }
}
