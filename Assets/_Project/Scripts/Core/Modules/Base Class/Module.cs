using _Project.Scripts.Core.AudioPooling;
using _Project.Scripts.Core.Modules.Interface;
using _Project.Scripts.Core.Player;
using Sisus.Init;
using UnityEngine;

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
        [TextArea]public string description = "If this module is player-placeable define this";
        public Sprite moduleSprite;

        [TextArea]public string description = "desciption tbd";

        public bool EnableModule { get; set; } = true;
        
        private ModuleState _previousState = ModuleState.None;
        public ModuleState state = ModuleState.Load;
        protected AudioPooler _audioPooler;
        
        [Header("Audio")]
        [SerializeField] protected AudioClip fastForwardSound;
        [SerializeField] protected float fastForwardSoundVolume = 1f;
        [SerializeField] protected AudioClip rewindSound;
        [SerializeField] protected float rewindSoundVolume = 1f;

        protected override void Init(AudioPooler audioPooler)
        {
            _audioPooler = audioPooler;
        }

        protected void ActByState()
        {
            if (_previousState != state)
            {
                OnStateChanged(_previousState);
                _previousState = state;
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
            if (EnableModule)
                ActByState();
        }
        
        protected abstract void LoadState();
        protected abstract void AttackState();
        protected abstract void UsedState();
        protected abstract void OnStateChanged(ModuleState prevState);
        public abstract void Rewind();
        public abstract void FastForward();
        public abstract void CancelRewind();
        public abstract void CancelFastForward();
        public abstract void ShowVisual(PlayerData.PlayerID playerID);
        public abstract void HideVisual(PlayerData.PlayerID playerID);
    }
}
