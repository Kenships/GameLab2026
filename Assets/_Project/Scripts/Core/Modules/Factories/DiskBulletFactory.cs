using _Project.Scripts.Util.ExtensionMethods;
using UnityEngine;

namespace _Project.Scripts.Core.Modules.Factories
{
    [CreateAssetMenu(menuName = "BulletFactories/DiskBulletFactory", fileName = "DiskBulletFactory")]
    public class DiskBulletFactory : BulletFactoryBase
    {
        // projectile lifetime: 3.5f
        // Bullet Speed: 10f
        // Pierce: 3
        
        public override GameObject CreateBullet(Transform target)
        {
            GameObject bullet = Instantiate(bulletPrefab);
            
            RecordDiscBullet recordDiscBullet = bullet.GetOrAdd<RecordDiscBullet>();
            recordDiscBullet.Initialize(target, targetLayer, inflictor, projectileSpeed, projectileLifetime, pierce);
            return bulletPrefab;
        }
    }
}
