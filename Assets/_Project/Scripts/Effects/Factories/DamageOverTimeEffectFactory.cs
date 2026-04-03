using System;
using _Project.Scripts.Core.Enemies;
using _Project.Scripts.Core.HealthManagement;
using _Project.Scripts.Effects.Interface;
using _Project.Scripts.Effects.Runtime;
using UnityEngine;

namespace _Project.Scripts.Effects.Factories
{
    [Serializable]
    public class DamageOverTimeEffectFactory: IEffectFactory<IDamageable>
    {
        [SerializeField] private DamageOverTimeEffect.DamageOverTimeType damageOverTimeType;
        [SerializeField] private float duration;
        [SerializeField] private float tickInterval;
        [SerializeField] private float damagePerTick;
        
        [SerializeField] private GameObject vfx;
        
        
        public IEffect<IDamageable> CreateEffect()
        {
            return new DamageOverTimeEffect
                   {
                       Type = damageOverTimeType,
                       Duration = duration,
                       TickInterval = tickInterval,
                       DamagePerTick = damagePerTick,
                       Vfx = vfx
                   };
        }
    }
    
    [Serializable]
    public class EnemyDamageOverTimeEffectFactory: IEffectFactory<EnemyBase>
    {
        [SerializeField] private DamageOverTimeEffect.DamageOverTimeType damageOverTimeType;
        [SerializeField] private float duration;
        [SerializeField] private float tickInterval;
        [SerializeField] private float damagePerTick;
        
        [SerializeField] private GameObject vfx;
        
        
        public IEffect<EnemyBase> CreateEffect()
        {
            return new DamageOverTimeEffect
            {
                Type = damageOverTimeType,
                Duration = duration,
                TickInterval = tickInterval,
                DamagePerTick = damagePerTick,
                Vfx = vfx
            };
        }
    }
}
