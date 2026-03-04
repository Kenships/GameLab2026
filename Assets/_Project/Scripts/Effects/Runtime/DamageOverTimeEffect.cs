using _Project.Scripts.Core.HealthManagement;
using _Project.Scripts.Effects.Interface;
using _Project.Scripts.Util.Timer.Timers;
using UnityEngine.Events;

namespace _Project.Scripts.Effects.Runtime
{
    public class DamageOverTimeEffect : IEffect<IDamageable>
    {
        public float Duration;
        public float TickInterval;
        public float DamagePerTick;
        
        private IntervalTimer _timer;
        private IDamageable _target;
        
        public UnityAction<IEffect<IDamageable>> OnComplete { get; set; }
        public void Apply(IDamageable target)
        {
            _target = target;
            _timer = new IntervalTimer(Duration, TickInterval);
            _timer.OnInterval = OnInterval;
            _timer.OnTimerEnd = OnStop;
            _timer.Start();
        }
        
        public void Cancel()
        {
            _timer?.Stop();
            CleanUp();
        }

        private void OnInterval() => _target.Damage(DamagePerTick);
        void OnStop() => CleanUp();

        private void CleanUp()
        {
            _timer = null;
            _target = null;
        }
    }
}
