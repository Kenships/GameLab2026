using UnityEngine;

namespace _Project.Scripts.Util.ExtensionMethods
{
    public static class ColliderExtensions
    {
        public static bool IsOnLayer(this Collider collider, LayerMask mask)
        {
            int colliderLayerBit = 1 << collider.gameObject.layer;
            return (mask.value & colliderLayerBit) != 0;
        }
    }
}
