using System;
using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Core.Modules.Base_Class;
using _Project.Scripts.Core.Player;
using Obvious.Soap;
using UnityEngine;

namespace _Project.Scripts.Core.Modules
{
    public class ModuleRegistry : MonoBehaviour
    {
        [SerializeField] private ScriptableEventNoParam perfectWave;
        [SerializeField] private ScriptableEventGameObject moduleSpawnedEvent;
        [SerializeField] private List<Module> _modules = new();

        private void Start()
        {
            moduleSpawnedEvent.OnRaised += ModuleSpawnedEventOnRaised;
            perfectWave.OnRaised += RewindAll;
            
            Module[] modules = FindObjectsByType<Module>(FindObjectsSortMode.None);
            
            foreach (Module module in modules)
            {
                if (module is Car)
                {
                    continue;
                }
                
                _modules.Add(module);
            }
        }

        private void OnDestroy()
        {
            moduleSpawnedEvent.OnRaised -= ModuleSpawnedEventOnRaised;
            perfectWave.OnRaised -= RewindAll;
        }

        private void ModuleSpawnedEventOnRaised(GameObject obj)
        {
            if (!obj.TryGetComponent(out Module module))
            {
                Debug.LogError("Module spawned is not a module.");
                return;
            }
            
            _modules.Add(module);
        }
        
        
        public void RewindAll()
        {
            StartCoroutine(RewindRoutine());
        }

        private IEnumerator RewindRoutine()
        {
            float timer = 2;
            while (timer > 0)
            {
                foreach (var module in _modules)
                {
                    module.Rewind(PlayerData.PlayerID.Player1);
                }
                timer -= Time.deltaTime;
                yield return null;
            }

            foreach (var module in _modules)
            {
                module.CancelRewind(PlayerData.PlayerID.Player1);
            }
        }
    }
}
