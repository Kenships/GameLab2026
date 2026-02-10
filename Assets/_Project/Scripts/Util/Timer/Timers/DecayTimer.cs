using UnityEngine;

namespace _Project.Scripts.Util.Timer.Timers
{
    /// <summary>
    /// Timer that counts Up to a specific value but decays when turned off
    /// </summary>
    public class DecayTimer : Timer
    {
        private const float DefaultDecayRate = 0.5f;
        private readonly float _decayRate;
        private readonly float _duration;
        
        public DecayTimer(float value, float decayRate = DefaultDecayRate) : base(0)
        {
            _decayRate = decayRate;
            _duration = value;
        }

        public override void Tick()
        {
            if (IsRunning) {
                CurrentTime += Time.deltaTime;
            }
            else
            {
                CurrentTime -= Time.deltaTime * _decayRate;
            }

            if (CurrentTime < 0)
            {
                Reset();
                TimerManager.DeregisterTimer(this);
            }

            if (CurrentTime >= _duration)
            {
                Stop();
            }
        }

        public override bool IsFinished => CurrentTime >= _duration;
        public override float Progress => Mathf.Clamp(CurrentTime / _duration, 0f, 1f);
    }
}
