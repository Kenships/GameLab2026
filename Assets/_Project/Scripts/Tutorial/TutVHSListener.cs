using System;
using _Project.Scripts.Core;
using _Project.Scripts.Core.HealthManagement;
using _Project.Scripts.Core.Modules;
using Obvious.Soap;
using UnityEngine;

namespace _Project.Scripts.Tutorial
{
    public class TutVHSListener : MonoBehaviour, ITutorialListener
    {
        [SerializeField] private VHSModule vhsModule;
        
        private Action _callback;
        
        private Health _health;
        
        private void Awake()
        {
            _health = vhsModule.GetComponent<Health>();
        }

        private void OnDestroy()
        {
            _health.OnFullHp -= VhsHPFullOnRaised;
        }

        public void Invoke(Action callback)
        {
            _callback = callback;
            
            vhsModule.EnableModule = true;
            _health.OnFullHp += VhsHPFullOnRaised;
        }

        private void VhsHPFullOnRaised()
        {
            _health.OnFullHp -= VhsHPFullOnRaised;
            _callback?.Invoke();
        }
    }
}
