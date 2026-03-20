using UnityEngine;

namespace _Project.Scripts.Util
{
    public static class LayerMaskHelper
    {
        public static int ToLayerIndex(this LayerMask layerMask)
        {
            return (int)Mathf.Log(layerMask.value, 2);
        }

        public static string ToLayerName(this LayerMask layerMask)
        {
            int index = layerMask.ToLayerIndex();
            return LayerMask.LayerToName(index);
        }
    }
}
