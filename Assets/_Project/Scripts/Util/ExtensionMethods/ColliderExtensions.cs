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

        public static bool IsOnLayer(this Collider collider, LayerMask[] masks)
        {
            foreach (var mask in masks)
            {
                if(!collider.IsOnLayer(mask)) return false;
            }
            return true;
        }
    }
}
