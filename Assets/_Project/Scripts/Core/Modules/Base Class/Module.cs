using _Project.Scripts.Core.AudioPooling;
using _Project.Scripts.Core.Modules.Interface;
using _Project.Scripts.Core.Player;
using Sisus.Init;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Project.Scripts.Core.Modules.Base_Class
{
    public abstract class Module : MonoBehaviour<AudioPooler>, ITimeControllable, IVisualSelectable
    {
        public enum ModuleState
        {
            None,
            Load,
            Attack,
            Used
        }
        
        private ModuleState _previousState = ModuleState.None;
        public ModuleState state = ModuleState.Load;
        protected AudioPooler _audioPooler;

        protected override void Init(AudioPooler audioPooler)
        {
            _audioPooler = audioPooler;
        }

        protected void ActByState()
        {
            if (_previousState != state)
            {
                _previousState = state;
                OnStateChanged(state);
            }
            
            switch (state)
            {
                case ModuleState.Load:
                    LoadState();
                    break;
                case ModuleState.Attack:
                    AttackState();
                    break;
                case ModuleState.Used:
                    UsedState();
                    break;
            }
        }

        protected virtual void FixedUpdate()
        {
            ActByState();
        }
        
        protected abstract void LoadState();
        protected abstract void AttackState();
        protected abstract void UsedState();
        protected abstract void OnStateChanged(ModuleState newState);
        public abstract void Rewind();
        public abstract void FastForward();
        public abstract void CancelRewind();
        public abstract void CancelFastForward();
        public abstract void ShowVisual(PlayerData.PlayerID playerID);
        public abstract void HideVisual(PlayerData.PlayerID playerID);
    }
}
