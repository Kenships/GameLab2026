using _Project.Scripts.Core.Enemies;
using _Project.Scripts.Effects.Interface;
using _Project.Scripts.Util.Timer.Timers;
using UnityEngine.Events;

namespace _Project.Scripts.Effects.Runtime
{
    public class StunEnemyEffect : IEffect<EnemyBase>
    {
        public float Duration;
        
        public UnityAction<IEffect<EnemyBase>> OnComplete { get; set; }
        
        private CountdownTimer _timer;
        private EnemyBase _target;

        public void Apply(EnemyBase target)
        {
            _target = target;
            
            if (_target.Stunned) // The current implementation does not allow for stuns to stack
                return;
            
            target.Stunned = true;
            
            _timer = new CountdownTimer(Duration);
            _timer.OnTimerEnd += CleanUp;
            _timer.Start();
        }

        public void Cancel()
        {
            CleanUp();
        }
        
        private void CleanUp()
        {
            if (_target)
            {
                _target.Stunned = false;
            }
            _timer = null;
            _target = null;
            OnComplete?.Invoke(this);
        }
    }
}
