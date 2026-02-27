using _Project.Scripts.Core.Modules.Interface;
using UnityEngine;

namespace _Project.Scripts.Core.Modules.Base_Class
{
    public abstract class Module : MonoBehaviour, ITimeControllable
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
        

        protected void ActByState()
        {
            if (_previousState != state)
            {
                _previousState = state;
                OnStateChanged(_previousState);
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
    }
}
