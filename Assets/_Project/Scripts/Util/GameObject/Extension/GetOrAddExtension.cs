using UnityEngine;

namespace _Project.Scripts.Util.GameObject.Extension
{
    public static class GetOrAddExtension
    {
        public static T GetOrAdd<T>(this UnityEngine.GameObject gameObject) where T : Component
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
