using System;

namespace _Project.Scripts.Util.Timer {
    public abstract class Timer : IDisposable {
        public float CurrentTime { get; protected set; }
        public bool IsRunning { get; private set; }
        public bool IsPaused { get; private set; }

        protected float _initialTime;
        

        public Action OnTimerRaised = delegate { };
        public Action OnTimerEnd = delegate { };

        protected Timer(float value) {
            _initialTime = value;
            IsPaused = false;
        }

        public void Start() {
            
            if (IsPaused && !IsFinished)
            {
                Resume();
                return;
            }
            
            CurrentTime = _initialTime;
            if (!IsRunning) {
                IsPaused = false;
                IsRunning = true;
                TimerManager.RegisterTimer(this);
                OnTimerRaised.Invoke();
            }
        }

        public void Stop() {
            if (IsRunning)
            {
                IsPaused = false;
                IsRunning = false;
                TimerManager.DeregisterTimer(this);
                OnTimerEnd.Invoke();
            }
        }

        public abstract void Tick();
        public abstract bool IsFinished { get; }
        public abstract float Progress { get; }
        public void Resume()
        {
            IsPaused = false;
            IsRunning = true;
        }

        public void Pause()
        {
            if (IsRunning)
            {
                IsPaused = true;
                IsRunning = false;
            }
        }

        public virtual void Reset()
        {
            IsRunning = false;
            IsPaused = false;
            CurrentTime = _initialTime;
        }

        public virtual void Reset(float newTime) {
            _initialTime = newTime;
            Reset();
            //Questionable reset that may cause overhead when there are a lot of Timers active
            TimerManager.DeregisterTimer(this);
            Start();
        }

        bool disposed;

        ~Timer() {
            Dispose(false);
        }

        // Call Dispose to ensure deregistration of the timer from the TimerManager
        // when the consumer is done with the timer or being destroyed
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if (disposed) return;

            if (disposing) {
                TimerManager.DeregisterTimer(this);
            }

            disposed = true;
        }
    }
}
