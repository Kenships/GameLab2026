using System.Collections.Generic;

namespace _Project.Scripts.Targeting
{
    public interface ITargetingFilter<T>
    {
        List<T> Filter(List<T> targets);
    }
}
