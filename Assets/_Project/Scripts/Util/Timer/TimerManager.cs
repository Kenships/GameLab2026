using System.Collections.Generic;
using _Project.Scripts.Util.Timer.Extensions;

namespace _Project.Scripts.Util.Timer {
    public static class TimerManager {
        static readonly List<Timer> _timers = new();
        static readonly List<Timer> _sweep = new();
        
        public static void RegisterTimer(Timer timer) => _timers.Add(timer);
        public static void DeregisterTimer(Timer timer) => _timers.Remove(timer);

        public static void UpdateTimers() {
            if (_timers.Count == 0) return;
            
            _sweep.RefreshWith(_timers);
            foreach (var timer in _sweep) {
                timer.Tick();
            }
        }
        
        public static void Clear() {
            _sweep.RefreshWith(_timers);
            foreach (var timer in _sweep) {
                timer.Dispose();
            }
            
            _timers.Clear();
            _sweep.Clear();
        }
    }
}
