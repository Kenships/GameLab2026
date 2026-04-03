using _Project.Scripts.Core.Modules.Interface;
using _Project.Scripts.Core.Player;
using UnityEngine;

namespace _Project.Scripts.Core.Modules.Base_Class
{
    public abstract class Module : TimeControllableBase, IVisualSelectable
    {
        [SerializeField] protected ParticleSystem brokenEffect;
        // no broken effect for first state change
        protected bool allowBrokenEffect = false;
        public enum ModuleState
        {
            None,
            Load,
            Attack,
            Used
        }
        [TextArea]public string description = "If this module is player-placeable define this";
        public Sprite moduleSprite;
        
        public bool EnableModule { get; set; } = true;
        
        private ModuleState _previousState = ModuleState.None;
        public ModuleState state = ModuleState.Load;

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
        public abstract void ShowVisual(PlayerData.PlayerID playerID);
        public abstract void HideVisual(PlayerData.PlayerID playerID);
    }
}
