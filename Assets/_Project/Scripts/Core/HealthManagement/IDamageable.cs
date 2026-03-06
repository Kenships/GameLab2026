using _Project.Scripts.Effects.Interface;

namespace _Project.Scripts.Core.HealthManagement
{
    public interface IDamageable
    {
        void Damage(float damage);
        void ApplyEffect(IEffect<IDamageable> effect);
        void RemoveEffect(IEffect<IDamageable> effect);
    }
}
