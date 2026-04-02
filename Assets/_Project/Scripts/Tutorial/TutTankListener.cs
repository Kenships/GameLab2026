using System;
using _Project.Scripts.Core.HealthManagement;
using UnityEngine;

namespace _Project.Scripts.Tutorial
{
    public class TutTankListener : MonoBehaviour, ITutorialListener
    {
        [SerializeField] private Health tankHealth;
        
        private Action _callback;
        public void Invoke(Action callback)
        {
            _callback = callback;
            tankHealth.OnDeath += OnDeath;
        }
        
        private void OnDestroy()
        {
            tankHealth.OnDeath -= OnDeath;
        }

        private void OnDeath()
        {
            tankHealth.OnDeath -= OnDeath;
            _callback?.Invoke();
        }
    }
}
