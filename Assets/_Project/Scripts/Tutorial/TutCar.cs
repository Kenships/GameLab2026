using System;
using _Project.Scripts.Core.Modules;

namespace _Project.Scripts.Tutorial
{
    public class TutCar : Car
    {
        public Action OnCarAttack;
        protected override void PerformAttack()
        {
            OnCarAttack?.Invoke();
            base.PerformAttack();
        }
    }
}
