namespace _Project.Scripts.Effects.Interface
{
    public interface IEffectFactory<TTarget>
    {
        IEffect<TTarget> CreateEffect();
    }
}
