using _Project.Scripts.Core.Enemies;
using _Project.Scripts.Effects.Interface;
using _Project.Scripts.Util.Timer.Timers;
using UnityEngine;
using UnityEngine.Events;

namespace _Project.Scripts.Effects.Runtime
{
    public class SlowEnemyEffect : IEffect<EnemyBase>
    {
        public enum SlowType
        {
            SourceOfSlow1,
            SourceOfSlow2,
        } 
        
        public SlowType Type;
        public float Duration;
        public float SlowPercentageFactor;
        
        public UnityAction<IEffect<EnemyBase>> OnComplete { get; set; }
        
        [SerializeField] private CountdownTimer _timer;
        private EnemyBase _target;
        
        public void Apply(EnemyBase target)
        {
            if (SlowPercentageFactor >= 100f)
            {
                Debug.LogError("SlowFactor cannot greater than 100%. Please Use Stun Effect instead.");
                return;
            }
            
            _target = target;
            _target.SpeedMultiplier *= 1f - SlowPercentageFactor / 100f;
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
                _target.SpeedMultiplier /= 1f - SlowPercentageFactor / 100f;
            }
            _target = null;
            _timer = null;
            OnComplete?.Invoke(this);
        }
        
        public float SlowPotential => SlowPercentageFactor * _timer?.CurrentTime ?? SlowPercentageFactor * Duration;
    }
}
