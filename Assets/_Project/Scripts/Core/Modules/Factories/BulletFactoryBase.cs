using _Project.Scripts.Effects.Inflictors;
using UnityEngine;

namespace _Project.Scripts.Core.Modules.Factories
{
    public abstract class BulletFactoryBase : ScriptableObject
    {
        [SerializeField] protected LayerMask targetLayer;
        [SerializeField] protected GameObject bulletPrefab;
        [SerializeField] protected EnemyEffectInflictor inflictor;
        [SerializeField] protected float projectileSpeed;
        [SerializeField] protected float projectileLifetime;
        [SerializeField] protected int pierce;
        
        public abstract GameObject CreateBullet(Transform target);
    }
}
