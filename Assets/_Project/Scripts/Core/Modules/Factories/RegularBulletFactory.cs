using _Project.Scripts.Util.ExtensionMethods;
using UnityEngine;

namespace _Project.Scripts.Core.Modules.Factories
{
    [CreateAssetMenu(menuName = "BulletFactories/RegularBulletFactory", fileName = "RegularBulletFactory")]
    public class RegularBulletFactory : BulletFactoryBase
    {
        public override GameObject CreateBullet(Transform target)
        {
            GameObject bullet = Instantiate(bulletPrefab);
            RegularBullet recordDiscBullet = bullet.GetOrAdd<RegularBullet>();
            recordDiscBullet.Initialize(targetLayer, inflictor, projectileSpeed, projectileLifetime, pierce);
            return bulletPrefab;
        }
    }
}
