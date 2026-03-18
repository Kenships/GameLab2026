using _Project.Scripts.Util.ExtensionMethods;
using UnityEngine;

namespace _Project.Scripts.Core.Modules.Factories
{
    [CreateAssetMenu(menuName = "BulletFactories/RegularBulletFactory", fileName = "RegularBulletFactory")]
    public class RegularBulletFactory : BulletFactoryBase
    {
        public override GameObject CreateBullet(Transform target, Vector3 position, Quaternion rotation)
        {
            GameObject bullet = Instantiate(bulletPrefab, position, rotation);
            RegularBullet recordDiscBullet = bullet.GetOrAdd<RegularBullet>();
            recordDiscBullet.Initialize(targetLayer, inflictor, projectileSpeed, projectileLifetime, pierce);
            return bulletPrefab;
        }
    }
}
