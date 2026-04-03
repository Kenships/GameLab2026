using System;
using _Project.Scripts.Effects.Interface;

namespace _Project.Scripts.Core.HealthManagement
{
    public interface IDamageable
    {
        void Damage(float damage);
        void ApplyEffect<T>(IEffect<T> effect) where T : IDamageable;
        void RemoveEffect(Guid effect);
    }
}
