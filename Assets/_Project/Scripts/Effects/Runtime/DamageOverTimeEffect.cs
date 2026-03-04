using _Project.Scripts.Core.HealthManagement;
using _Project.Scripts.Effects.Interface;
using _Project.Scripts.Util.Timer.Timers;
using UnityEngine;
using UnityEngine.Events;

namespace _Project.Scripts.Effects.Runtime
{
    public class DamageOverTimeEffect : IEffect<IDamageable>
    {
        public enum DamageOverTimeType
        {
            Burning,
            Poison
        }

        public DamageOverTimeType Type;

        public float Duration;
        public float TickInterval;
        public float DamagePerTick;

        [SerializeField] private IntervalTimer _timer;
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
            OnComplete?.Invoke(this);
        }

        public float DamagePotential => _timer != null 
            ? DamagePerTick * (int)(_timer.CurrentTime / TickInterval) 
            : DamagePerTick * (int) (Duration / TickInterval);
        public float TimeUntilNextTick => _timer != null ? 
            _timer.TimeUntilNextInerval
            : TickInterval;

        public static bool ReplaceAndCancelWithBest(DamageOverTimeEffect oldDotEffect,
            DamageOverTimeEffect newDotEffect, out DamageOverTimeEffect bestEffect)
        {
            // Allows only one dot of the same type to be active at a time
            // Replaces old effect with a new effect if it exists
            bestEffect = oldDotEffect;
            
            if (oldDotEffect.Type != newDotEffect.Type)
            {
                return false;
            }

            if (newDotEffect.DamagePotential <= oldDotEffect.DamagePotential)
            {
                // The new effect is not worth it
                return false;
            }

            // If there is progress to the next tick with the old effect, give the new effect a headstart.
            float minTimeUntilNextTick = Mathf.Min(oldDotEffect.TimeUntilNextTick, newDotEffect.TimeUntilNextTick);
            newDotEffect.Duration = newDotEffect.Duration - newDotEffect.TickInterval + minTimeUntilNextTick;

            // Cancel the old effect
            oldDotEffect.Cancel();

            // Apply the new effect
            bestEffect = newDotEffect;
            return true;
        }
    }
}
