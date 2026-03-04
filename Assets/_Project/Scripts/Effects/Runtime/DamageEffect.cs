using _Project.Scripts.Core.HealthManagement;
using _Project.Scripts.Effects.Interface;
using UnityEngine.Events;

namespace _Project.Scripts.Effects.Runtime
{
    public class DamageEffect : IEffect<IDamageable>
    {
        public float Damage;
        
        public UnityAction<IEffect<IDamageable>> OnComplete { get; set; }

        public void Apply(IDamageable target)
        {
            target.Damage(10);
            OnComplete?.Invoke(this);
        }

        public void Cancel()
        {
            OnComplete?.Invoke(this);
        }
    }
}
