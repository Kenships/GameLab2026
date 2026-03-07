using System.Collections.Generic;
using _Project.Scripts.Targeting.Interface;

namespace _Project.Scripts.Targeting.Filters
{
    [System.Serializable]
    public class FilterAll<T> : ITargetingFilter<T>
    {
        public List<T> Filter(List<T> targets)
        {
            return targets;
        }
    }
}
