using _Project.Scripts.Core.Enemies;
using _Project.Scripts.Effects.Interface;
using _Project.Scripts.Util.Timer.Timers;
using UnityEngine;
using UnityEngine.Events;

namespace _Project.Scripts.Effects.Runtime
{
    public class SlowEnemyEffect : IEffect<EnemyBase>
    {
        public float Duration;
        public float SlowFactor;
        
        public UnityAction<IEffect<EnemyBase>> OnComplete { get; set; }
        
        private CountdownTimer _timer;
        private EnemyBase _target;
        
        public void Apply(EnemyBase target)
        {
            if (SlowFactor == 0f)
            {
                Debug.LogError("SlowFactor cannot be 0. Please Use Stun Effect instead.");
                return;
            }
            
            _target = target;
            _target.SpeedMultiplier *= SlowFactor;
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
                _target.SpeedMultiplier /= SlowFactor;
            }
            _target = null;
            _timer = null;
            OnComplete?.Invoke(this);
        }
    }
}
