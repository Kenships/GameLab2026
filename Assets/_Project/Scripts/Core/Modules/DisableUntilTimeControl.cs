using System;
using _Project.Scripts.Core.Modules.Base_Class;
using _Project.Scripts.Core.Modules.Interface;
using _Project.Scripts.Core.Player;
using UnityEngine;
using UnityEngine.Events;

namespace _Project.Scripts.Core.Modules
{
    public class DisableUntilTimeControl : MonoBehaviour, ITimeControllable
    {
        private enum TimeControlType
        {
            FastForward,
            Rewind
        }
        
        public UnityEvent enableEvent;
        [SerializeField] private TimeControlType timeControlType;
        private Module _module;
        
        private void Awake()
        {
            _module = GetComponentInParent<Module>();
            _module.EnableModule = false;
        }

        public void Rewind(PlayerData.PlayerID playerID)
        {
            if (timeControlType == TimeControlType.FastForward)
            {
                return;
            }
            
            _module.EnableModule = true;
            enableEvent?.Invoke();
            gameObject.SetActive(false);
        }

        public void CancelRewind(PlayerData.PlayerID playerID)
        {
            
        }

        public void FastForward(PlayerData.PlayerID playerID)
        {
            if (timeControlType == TimeControlType.Rewind)
            {
                return;
            }
            
            _module.EnableModule = true;
            enableEvent?.Invoke();
            gameObject.SetActive(false);
        }

        public void CancelFastForward(PlayerData.PlayerID playerID)
        {
            
        }
    }
}
