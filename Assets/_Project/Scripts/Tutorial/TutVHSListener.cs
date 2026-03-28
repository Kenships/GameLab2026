using System;
using _Project.Scripts.Core;
using _Project.Scripts.Core.Modules;
using Obvious.Soap;
using UnityEngine;

namespace _Project.Scripts.Tutorial
{
    public class TutVHSListener : MonoBehaviour, ITutorialListener
    {
        [SerializeField] private VHSModule vhsModule;
        [SerializeField] private ScriptableEventNoParam vhsHPFull;
        
        private Action _callback;
        
        private void Awake()
        {
            vhsModule.EnableModule = false;
        }

        private void OnDestroy()
        {
            vhsHPFull.OnRaised -= VhsHPFullOnRaised;
        }

        public void Invoke(Action callback)
        {
            _callback = callback;
            
            vhsModule.EnableModule = true;
            vhsHPFull.OnRaised += VhsHPFullOnRaised;
        }

        private void VhsHPFullOnRaised()
        {
            vhsHPFull.OnRaised -= VhsHPFullOnRaised;
            _callback?.Invoke();
        }
    }
}
