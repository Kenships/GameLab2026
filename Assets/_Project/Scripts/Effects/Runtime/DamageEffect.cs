using System;
using _Project.Scripts.Core.HealthManagement;
using _Project.Scripts.Effects.Interface;
using UnityEngine;
using UnityEngine.Events;

namespace _Project.Scripts.Effects.Runtime
{
    public class DamageEffect : IEffect<IDamageable>
    {
        public float Damage;

        public Guid InstanceID { get; } = Guid.NewGuid();
        public UnityAction<Guid> OnComplete { get; set; }
        public GameObject Vfx { get; set; }

        public void Apply(IDamageable target)
        {
            target.Damage(Damage);
            OnComplete?.Invoke(InstanceID);
        }

        public void Cancel()
        {
            OnComplete?.Invoke(InstanceID);
        }
    }
}
