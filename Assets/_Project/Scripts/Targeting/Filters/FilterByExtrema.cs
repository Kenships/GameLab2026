using System;
using System.Collections.Generic;
using _Project.Scripts.Core.Enemies;
using _Project.Scripts.Targeting.Interface;
using _Project.Scripts.Util.CustomAttributes;
using UnityEngine;

namespace _Project.Scripts.Targeting.Filters
{
    public abstract class FilterByExtrema<T> : ITargetingFilter<T>
    {
        protected enum FilterMode
        {
            Highest,
            Lowest,
            Above,
            Below
        }
        
        [SerializeField] protected FilterMode mode;
        
        private bool IsHighestOrLowest => mode is FilterMode.Highest or FilterMode.Lowest;
        private bool IsAboveOrBelow => mode is FilterMode.Above or FilterMode.Below;
        
        [SerializeField, ShowIf(nameof(IsHighestOrLowest))] protected int amount;
        [SerializeField, ShowIf(nameof(IsAboveOrBelow))] protected float value;
        
        public List<T> Filter(List<T> targets)
        {
            switch (mode)
            {
                case FilterMode.Highest:
                    return FilterExtrema(targets, GreaterThan);
                case FilterMode.Lowest:
                    return FilterExtrema(targets, LessThan);
                case FilterMode.Above:
                    return FilterThreshold(targets, GreaterThanThreshold);
                case FilterMode.Below:
                    return FilterThreshold(targets, LessThanThreshold);
            }
            
            return new List<T>();
        }
        
        protected abstract bool GreaterThan(T a, T b);
        protected abstract bool LessThan(T a, T b);
        protected abstract bool GreaterThanThreshold(T a, float threshold);
        protected abstract bool LessThanThreshold(T a, float threshold);
        
        public List<T> FilterExtrema(List<T> targets, Func<T, T, bool> compare)
        {
            return SortingUtil.GetFirstN(amount, targets, compare);
        }

        public List<T> FilterThreshold(List<T> targets, Func<T, float, bool> compare) 
        {
            List<T> filtered = new List<T>();
            foreach (T target in targets)
            {
                if (compare(target, value))
                {
                    filtered.Add(target);
                }
            }
            return filtered;
        }
    }
}
