using System;
using Obvious.Soap;
using UnityEngine;

namespace _Project.Scripts.Tutorial
{
    public class TutModuleSpawnListener : MonoBehaviour, ITutorialListener
    {
        [SerializeField] private ScriptableEventGameObject spawnEvent;
        
        private Action _callback;
        
        public void Invoke(Action callback)
        {
            _callback = callback;
            spawnEvent.OnRaised += SpawnEventOnOnRaised;
        }

        private void OnDestroy()
        {
            spawnEvent.OnRaised -= SpawnEventOnOnRaised;
        }

        private void SpawnEventOnOnRaised(GameObject obj)
        {
            spawnEvent.OnRaised -= SpawnEventOnOnRaised;
            _callback?.Invoke();
        }
    }
}
