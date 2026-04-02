using System;
using _Project.Scripts.Core.Modules.Base_Class;
using _Project.Scripts.Core.Player;
using UnityEngine;

namespace _Project.Scripts.Tutorial
{
    public class TutPickupListener : MonoBehaviour, ITutorialListener
    {
        [SerializeField] private TutPlayerInteractionController player1;
        [SerializeField] private TutPlayerInteractionController player2;

        private bool _beginCheck;

        private Action _callback;

        private PickupModuleBase[] _modules;

        private bool _listening;

        public void Invoke(Action callback)
        {
            _modules = FindObjectsByType<PickupModuleBase>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            
            _callback = callback;
            _beginCheck = true;
        }

        private void OnDestroy()
        {
            _beginCheck = false;
        }

        private void Update()
        {
            if (!_beginCheck) return;

            foreach (PickupModuleBase module in _modules)
            {
                if (!module.EnableModule)
                {
                    return;
                }
            }
            
            _beginCheck = false;
            _callback?.Invoke();
        }
        
    }
}
