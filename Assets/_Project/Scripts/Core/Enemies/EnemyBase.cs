using System;
using System.Collections.Generic;
using _Project.Scripts.Core.AudioPooling;
using _Project.Scripts.Core.HealthManagement;
using _Project.Scripts.Effects.Interface;
using _Project.Scripts.Effects.Runtime;
using _Project.Scripts.Util.CustomAttributes;
using _Project.Scripts.Util.ExtensionMethods;
using Sisus.Init;
using UnityEngine;
using UnityEngine.AI;

namespace _Project.Scripts.Core.Enemies
{
    public abstract class EnemyBase : MonoBehaviour<AudioPooler>, IDamageable
    {
        [Header("Enemy Debug Values")]
        [field: SerializeField, ReadOnly] public bool Stunned { get; set; } = false;
        [field: SerializeField, ReadOnly] public float SpeedMultiplier { get; set; } = 1f;
        [field: SerializeField, ReadOnly] public NavMeshAgent Agent { get; set; }
        [SerializeReference, SubclassSelector] protected List<(IEffect<EnemyBase> effect, GameObject vfx)> enemyEffects = new();
        
        public float Health => _health.CurrentHealth;
        public float Speed => _moveSpeed * SpeedMultiplier;
        
        protected AudioPooler _audioPooler;
        protected Health _health;
        protected float _moveSpeed;
        
        protected override void Init(AudioPooler playerReader)
        {
            _audioPooler = playerReader;
            _health ??= gameObject.GetOrAdd<Health>();
            Agent ??= gameObject.GetOrAdd<NavMeshAgent>();
            _health.OnDeath += OnDeath;
        }

        public virtual void Damage(float damage)
        {
            _health.AddToHealth(-damage);
        }

        public virtual void ApplyEffect<T>(IEffect<T> effect) where T : IDamageable
        {
            if (effect is SlowEnemyEffect slowEffect)
            {
                AddSlowEffect(slowEffect);
            }
            else if (effect is DamageOverTimeEffect dotEffect)
            {
                AddDotEffect(dotEffect);
            }
            else
            {
                AddToEffects(effect as IEffect<EnemyBase>);
            }
        }

        public virtual void RemoveEffect(Guid guid) 
        {
            foreach (var (effect, vfx) in enemyEffects)
            {
                if (effect.InstanceID == guid)
                {
                    if (vfx)
                    {
                        Destroy(vfx);
                    }
                    
                    effect.OnComplete -= RemoveEffect;
                    enemyEffects.Remove((effect, vfx));
                    return;
                }
            }
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
            foreach (var (effect, vfx) in enemyEffects)
            {
                if (vfx)
                {
                    Destroy(vfx);
                }
                effect.OnComplete -= RemoveEffect;
                effect.Cancel();
            }
            
            enemyEffects.Clear();
        }
        
        #region Utility

        private void AddToEffects(IEffect<EnemyBase> effect)
        {
            if (effect == null)
            {
                Debug.LogError("Failed to Add Effect.");
                return;
            }
            
            effect.OnComplete += RemoveEffect;

            GameObject vfx = null;
            
            if (effect.Vfx)
            {
                vfx = Instantiate(effect.Vfx, transform);
            }

            IEffect<EnemyBase> enemyEffect = effect;
            
            enemyEffects.Add((enemyEffect, vfx));
            
            enemyEffect.Apply(this);
        }
        
        protected virtual void AddDotEffect(DamageOverTimeEffect newDotEffect)
        {
            // Allows only one dot of the same type to be active at a time
            // Replaces old effect with a new effect if it exists

            foreach (var (effect, vfx) in enemyEffects)
            {
                if (effect is DamageOverTimeEffect dotEffect && dotEffect.Type == newDotEffect.Type)
                {
                    // Returns whether the new effect replaced the old effect
                    if (!DamageOverTimeEffect.ReplaceAndCancelWithBest(dotEffect, newDotEffect, out var bestEffect))
                    {
                        return;
                    }

                    
                    Debug.Log($"Replaced {dotEffect.InstanceID} with {newDotEffect.InstanceID}");
                    AddToEffects(bestEffect);
                    
                    // We can return early as there will only be one DOT per type
                    return;
                }
            }
            Debug.Log($"Added {newDotEffect.InstanceID}");
            AddToEffects(newDotEffect);
        }
        
        private void AddSlowEffect(SlowEnemyEffect newSlowEffect)
        {
            foreach (var (effect, vfx) in enemyEffects)
            {
                if (effect is SlowEnemyEffect slowEffect && slowEffect.Type == newSlowEffect.Type)
                {
                    if (newSlowEffect.SlowPotential <= slowEffect.SlowPotential)
                    {
                        return;
                    }
                    
                    effect.Cancel();
                    AddToEffects(newSlowEffect);
                    return;
                }
            }
            
            AddToEffects(newSlowEffect);
        }
        
        #endregion
    }
}
