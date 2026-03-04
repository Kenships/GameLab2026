using System;
using UnityEngine;

namespace _Project.Scripts.Util.Timer.Timers
{
    [Serializable]
    public class IntervalTimer : Timer
    {
        private readonly float _negativeEpsilon = -0.005f;
        
        private float _interval;
        private float _nextInterval;
        
        public Action OnInterval = delegate { };

        public IntervalTimer(float totalTime, float intervalSeconds) : base(totalTime)
        {
            _interval = intervalSeconds;
            
            _nextInterval = (int) (totalTime/intervalSeconds) * intervalSeconds;
        }

        public override void Tick()
        {
            if (IsRunning && CurrentTime > 0)
            {
                CurrentTime -= Time.deltaTime;
            }

            while (CurrentTime <= _nextInterval && _nextInterval >= _negativeEpsilon)
            {
                OnInterval?.Invoke();
                _nextInterval -= _interval;
            }

            if (IsRunning && CurrentTime <= 0)
            {
                CurrentTime = 0;
                Stop();
            }
        }

        public override bool IsFinished => CurrentTime <= 0;
        public override float Progress => CurrentTime/_initialTime;
        public float TimeUntilNextInerval => CurrentTime - _nextInterval;
    }
}
