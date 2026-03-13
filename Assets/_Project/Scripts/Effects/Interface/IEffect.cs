using UnityEngine.Events;

namespace _Project.Scripts.Effects.Interface
{
    public interface IEffect<TTarget>
    {
        public UnityAction<IEffect<TTarget>> OnComplete { get; set; }
        void Apply(TTarget target);
        void Cancel();
    }
}
