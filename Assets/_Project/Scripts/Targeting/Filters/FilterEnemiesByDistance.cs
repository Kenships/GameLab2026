using _Project.Scripts.Core.Enemies;
using UnityEngine;

namespace _Project.Scripts.Targeting.Filters
{
    [System.Serializable]
    public class FilterEnemiesByDistance : FilterByDistance<EnemyBase>
    {
        protected override float GetDistance(Transform referencePos, EnemyBase enemy)
        {
            if (IsTargetingGameObject)
            {
                return base.GetDistance(referencePos, enemy);
            }
            
            var path = enemy.Agent.path;

            if (path.corners.Length < 2)
            {
                return 0f;
            }

            float totalDistance = 0f;

            for (int i = 0; i < path.corners.Length; i++)
            {
                totalDistance += Vector3.Distance(referencePos.position, path.corners[i]);
            }
            
            return totalDistance;
        }
    }
}
