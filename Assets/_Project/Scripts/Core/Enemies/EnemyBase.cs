using System;
using System.Collections.Generic;
using _Project.Scripts.Core.AudioPooling;
using _Project.Scripts.Core.HealthManagement;
using _Project.Scripts.Effects;
using _Project.Scripts.Effects.Interface;
using _Project.Scripts.Util.ExtensionMethods;
using Sisus.Init;

namespace _Project.Scripts.Core.Enemies
{
    public abstract class EnemyBase : MonoBehaviour<AudioPooler>, IDamageable
    {
        public bool Stunned { get; set; } = false;
        public float SpeedMultiplier { get; set; } = 1f;
        
        protected readonly List<IEffect<IDamageable>> _damageEffects = new();
        protected readonly List<IEffect<EnemyBase>> _enemyEffects = new();
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
            effect.OnComplete += RemoveEffect;
            _damageEffects.Add(effect);
            effect.Apply(this);
        }
        public virtual void RemoveEffect(IEffect<IDamageable> effect)
        {
            effect.OnComplete -= RemoveEffect;
            _damageEffects.Remove(effect);
        }

        public virtual void ApplyEffect(IEffect<EnemyBase> effect)
        {
            effect.OnComplete += RemoveEffect;
            _enemyEffects.Add(effect);
            effect.Apply(this);
        }
        
        public virtual void RemoveEffect(IEffect<EnemyBase> effect)
        {
            effect.OnComplete -= RemoveEffect;
            _enemyEffects.Remove(effect);
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
            foreach (var effect in _damageEffects)
            {
                effect.OnComplete -= RemoveEffect;
                effect.Cancel();
            }
            foreach (var effect in _enemyEffects)
            {
                effect.OnComplete -= RemoveEffect;
                effect.Cancel();
            }
        }
    }
}
