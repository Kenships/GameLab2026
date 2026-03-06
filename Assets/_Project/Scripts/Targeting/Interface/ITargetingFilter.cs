using System.Collections.Generic;

namespace _Project.Scripts.Targeting.Interface
{
    public interface ITargetingFilter<T>
    {
        List<T> Filter(List<T> targets);
    }
}
