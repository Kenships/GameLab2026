using System.Collections.Generic;

namespace _Project.Scripts.Targeting
{
    public interface ITargetingStrategy<T>
    {
        List<T> Evaluate(List<T> targets);
    }
}
