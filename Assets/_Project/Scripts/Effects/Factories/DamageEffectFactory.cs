using System;
using _Project.Scripts.Core.Enemies;
using _Project.Scripts.Core.HealthManagement;
using _Project.Scripts.Effects.Interface;
using _Project.Scripts.Effects.Runtime;
using UnityEngine;

namespace _Project.Scripts.Effects.Factories
{
    [Serializable]
    public class DamageEffectFactory : IEffectFactory<IDamageable>
    {
        [SerializeField] private float damage;
        
        public IEffect<IDamageable> CreateEffect()
        {
            return new DamageEffect { Damage = damage };
        }
    }
    
    [Serializable]
    public class EnemyDamageEffectFactory : IEffectFactory<EnemyBase>
    {
        [SerializeField] private float damage;
        
        public IEffect<EnemyBase> CreateEffect()
        {
            return new DamageEffect { Damage = damage };
        }
    }
}
