using System;
using UnityEngine;
using UnityEngine.Events;

namespace _Project.Scripts.Effects.Interface
{
    public interface IEffect<in TTarget>
    {
        public Guid InstanceID { get; }
        public UnityAction<Guid> OnComplete { get; set; }
        public GameObject Vfx { get; set; }
        
        
        void Apply(TTarget target);
        void Cancel();
    }
}
