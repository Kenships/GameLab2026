using UnityEngine;

namespace _Project.Scripts.Util.ExtensionMethods
{
    public static class GetOrAddExtension
    {
        public static T GetOrAdd<T>(this GameObject gameObject) where T : Component
        {
            T component = gameObject.GetComponent<T>();
            
            if (!component)
            {
                component = gameObject.AddComponent<T>();
            }

            return component;
        }
    }
}
