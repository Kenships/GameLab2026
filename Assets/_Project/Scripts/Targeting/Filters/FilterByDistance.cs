using System;
using System.Collections.Generic;
using _Project.Scripts.Core;
using _Project.Scripts.Core.Enemies;
using _Project.Scripts.Core.Modules;
using _Project.Scripts.Targeting.Interface;
using _Project.Scripts.Util.CustomAttributes;
using UnityEngine;

namespace _Project.Scripts.Targeting.Filters
{
    [Serializable]
    public class FilterByDistance<T> : ITargetingFilter<T> where T : MonoBehaviour
    {
        public enum DistanceOrigin
        {
            VHS,
            GameObject
        }

        public enum DistanceType
        {
            Closest,
            Farthest
        }
        
        protected bool IsTargetingGameObject => distanceOrigin == DistanceOrigin.GameObject;
        
        [SerializeField] private DistanceOrigin distanceOrigin;
        [SerializeField, ShowIf(nameof(IsTargetingGameObject))] private GameObject target;
        [SerializeField] private DistanceType findThe;
        [SerializeField] private int amount;
        
        public List<T> Filter(List<T> targets)
        {
            Transform origin = VHSModule.Location;
            switch (distanceOrigin)
            {
                case DistanceOrigin.VHS:
                    origin = VHSModule.Location;
                    break;
                
                case DistanceOrigin.GameObject:
                    origin = target.transform;
                    break;
            }
            
            bool LessThan(T a, T b) => a && b && GetDistance(origin, a) < GetDistance(origin, b);
            bool GreaterThan(T a, T b) => a && b && GetDistance(origin, a) > GetDistance(origin, b);

            Func<T, T, bool> compare = findThe == DistanceType.Closest ? LessThan : GreaterThan;
            
            return SortingUtil.GetFirstN(amount, targets, compare);
        }

        protected virtual float GetDistance(Transform referencePoint, T input)
        {
            Vector3 fromXZ = Vector3.ProjectOnPlane(referencePoint.position, Vector3.up);
            Vector3 toXZ = Vector3.ProjectOnPlane(input.transform.position, Vector3.up);
            return (toXZ - fromXZ).sqrMagnitude;
        }
    }
}
