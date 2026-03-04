using System;
using System.Collections.Generic;
using _Project.Scripts.Core.AudioPooling;
using _Project.Scripts.Core.HealthManagement;
using _Project.Scripts.Effects;
using _Project.Scripts.Effects.Interface;
using _Project.Scripts.Effects.Runtime;
using _Project.Scripts.Util.CustomAttributes;
using _Project.Scripts.Util.ExtensionMethods;
using Sisus.Init;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Project.Scripts.Core.Enemies
{
    public abstract class EnemyBase : MonoBehaviour<AudioPooler>, IDamageable
    {
        [Header("Enemy Debug Values")]
        [field: SerializeField, ReadOnly] public bool Stunned { get; set; } = false;
        [field: SerializeField, ReadOnly] public float SpeedMultiplier { get; set; } = 1f;
        [SerializeReference, SubclassSelector] protected List<IEffect<IDamageable>> damageEffects = new();
        [SerializeReference, SubclassSelector] protected List<IEffect<EnemyBase>> enemyEffects = new();
        
        
        protected AudioPooler _audioPooler;
        protected Health _health;
        
        protected override void Init(AudioPooler playerReader)
        {
            _audioPooler = playerReader;
            _health ??= gameObject.GetOrAdd<Health>();
            _health.OnDeath += OnDeath;
        }

        public virtual void Damage(float damage)
        {
            _health.AddToHealth(-damage);
        }

        public virtual void ApplyEffect(IEffect<IDamageable> effect)
        {
            if (effect is DamageOverTimeEffect dotEffect)
            {
                AddDotEffect(dotEffect);
            }
            else
            {
                effect.OnComplete += RemoveEffect;
                damageEffects.Add(effect);
                effect.Apply(this);
            }
        }
        
        protected virtual void AddDotEffect(DamageOverTimeEffect newDotEffect)
        {
            // Allows only one dot of the same type to be active at a time
            // Replaces old effect with a new effect if it exists

            foreach (var dot in damageEffects)
            {
                if (dot is DamageOverTimeEffect dotEffect && dotEffect.Type == newDotEffect.Type)
                {
                    // Returns whether the new effect replaced the old effect
                    if (!DamageOverTimeEffect.ReplaceAndCancelWithBest(dotEffect, newDotEffect, out var bestEffect))
                    {
                        Debug.Log("Don't Replace");
                        return;
                    }
                    
                    Debug.Log("Replace");
                    
                    bestEffect.OnComplete += RemoveEffect;
                    damageEffects.Add(bestEffect);
                    bestEffect.Apply(this);
                    // We can return early as there will only be one DOT per type
                    return;
                }
            }
            
            newDotEffect.OnComplete += RemoveEffect;
            damageEffects.Add(newDotEffect);
            newDotEffect.Apply(this);
        }
        
        public virtual void RemoveEffect(IEffect<IDamageable> effect)
        {
            effect.OnComplete -= RemoveEffect;
            damageEffects.Remove(effect);
        }

        public virtual void ApplyEffect(IEffect<EnemyBase> effect)
        {
            effect.OnComplete += RemoveEffect;
            enemyEffects.Add(effect);
            effect.Apply(this);
        }
        
        public virtual void RemoveEffect(IEffect<EnemyBase> effect)
        {
            effect.OnComplete -= RemoveEffect;
            enemyEffects.Remove(effect);
        }

        protected virtual void OnDeath()
        {
            if (gameObject != null)
            {
                Destroy(gameObject);
            }
        }

        protected void OnDestroy()
        {
            ClearEffects();
            _health.OnDeath -= OnDeath;
        }

        protected void ClearEffects()
        {
            foreach (var effect in damageEffects)
            {
                effect.OnComplete -= RemoveEffect;
                effect.Cancel();
            }
            foreach (var effect in enemyEffects)
            {
                effect.OnComplete -= RemoveEffect;
                effect.Cancel();
            }
        }
    }
}
