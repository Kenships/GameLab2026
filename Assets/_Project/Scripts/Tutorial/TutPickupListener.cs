using System;
using _Project.Scripts.Core.Modules.Base_Class;
using UnityEngine;

namespace _Project.Scripts.Tutorial
{
    public class TutPickupListener : MonoBehaviour, ITutorialListener
    {
        [SerializeField] private Module module1;
        [SerializeField] private Module module2;

        private bool _beginCheck;
        
        private Action _callback;
        
        public void Invoke(Action callback)
        {
            _callback = callback;
            
            _beginCheck = true;
        }

        private void Update()
        {
            if (!_beginCheck)
            {
                return;
            }

            if (module1.EnableModule && module2.EnableModule)
            {
                _beginCheck = false;
                _callback?.Invoke();
            }
        }
    }
}
