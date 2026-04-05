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
        [SerializeField] private ScriptableEventGameObject moduleSpawnedEvent;
        [SerializeField] private List<Module> _modules = new();

        private void Start()
        {
            moduleSpawnedEvent.OnRaised += ModuleSpawnedEventOnRaised;
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                if (!child.TryGetComponent(out Module module))
                {
                    continue;
                }
                
                if (module is Car or VHSModule)
                {
                    continue;
                }
                
                _modules.Add(module);
            }
        }

        private void ModuleSpawnedEventOnRaised(GameObject obj)
        {
            if (!obj.TryGetComponent(out Module module))
            {
                Debug.LogError("Module spawned is not a module.");
                return;
            }
            
            _modules.Add(module);
            RewindAll();
        }
        
        [ContextMenu("RewindAll")]
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
