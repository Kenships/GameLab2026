using System;
using _Project.Scripts.Core.Enemies;
using _Project.Scripts.Effects.Interface;
using _Project.Scripts.Effects.Runtime;
using UnityEngine;

namespace _Project.Scripts.Effects.Factories
{
    [Serializable]
    public class StunEnemyEffectFactory : IEffectFactory<EnemyBase>
    {
        [SerializeField] private float duration;
        [SerializeField] private GameObject vfx;
        
        public IEffect<EnemyBase> CreateEffect()
        {
            return new StunEnemyEffect
            {
                Duration = duration,
                Vfx = vfx
            };
        }
    }
}
