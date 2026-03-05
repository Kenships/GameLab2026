using System;
using System.Collections.Generic;
using _Project.Scripts.Core;
using _Project.Scripts.Core.Enemies;
using _Project.Scripts.Util.CustomAttributes;
using UnityEngine;

namespace _Project.Scripts.Targeting
{
    [Serializable]
    public struct FilterByDistance : ITargetingFilter<EnemyBase>
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
        
        private bool IsTargetingGameObject => distanceOrigin == DistanceOrigin.GameObject;
        
        [SerializeField] private DistanceOrigin distanceOrigin;
        [SerializeField, ShowIf(nameof(IsTargetingGameObject))] private GameObject target;
        [SerializeField] private DistanceType findThe;
        [SerializeField] private int amount;
        
        public List<EnemyBase> Filter(List<EnemyBase> targets)
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
            
            bool LessThan(EnemyBase a, EnemyBase b) => GetDistance(origin, a.transform) < GetDistance(origin, b.transform);
            bool GreaterThan(EnemyBase a, EnemyBase b) => GetDistance(origin, a.transform) > GetDistance(origin, b.transform);

            Func<EnemyBase, EnemyBase, bool> compare = findThe == DistanceType.Closest ? LessThan : GreaterThan;
            
            return SortingUtil.GetFirstN(amount, targets, compare);
        }

        private static float GetDistance(Transform startPos, Transform transform)
        {
            Vector3 fromXZ = Vector3.ProjectOnPlane(startPos.position, Vector3.up);
            Vector3 toXZ = Vector3.ProjectOnPlane(transform.position, Vector3.up);
            
            return (toXZ - fromXZ).sqrMagnitude;
        }
    }
}
