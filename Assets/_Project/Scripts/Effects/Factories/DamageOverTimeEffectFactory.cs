using System;
using _Project.Scripts.Core.HealthManagement;
using _Project.Scripts.Effects.Interface;
using _Project.Scripts.Effects.Runtime;
using UnityEngine;

namespace _Project.Scripts.Effects.Factories
{
    [Serializable]
    public class DamageOverTimeEffectFactory : IEffectFactory<IDamageable>
    {
        [SerializeField] private float duration;
        [SerializeField] private float tickInterval;
        [SerializeField] private float damagePerTick;
        
        
        public IEffect<IDamageable> CreateEffect()
        {
            return new DamageOverTimeEffect
                   {
                       Duration = duration,
                       TickInterval = tickInterval,
                       DamagePerTick = damagePerTick
                   };
        }
    }
}
