using System;
using _Project.Scripts.Core.HealthManagement;
using UnityEngine;

namespace _Project.Scripts.Core.Modules.Base_Class
{
    public abstract class Module : MonoBehaviour
    {
        public enum State
        {
            Load,
            Attack,
            Used
        }
        public State state = State.Load;

        protected void ActByState()
        {
            switch (state)
            {
                case State.Load:
                    LoadState();
                    break;
                case State.Attack:
                    AttackState();
                    break;
                case State.Used:
                    UsedState();
                    break;
            }
        }

        protected virtual void Update()
        {
            ActByState();
        }

        protected abstract void LoadState();
        protected abstract void AttackState();
        protected abstract void UsedState();
    }
}
