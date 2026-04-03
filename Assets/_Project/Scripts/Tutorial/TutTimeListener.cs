using System;
using _Project.Scripts.Util.Timer.Timers;
using UnityEngine;

namespace _Project.Scripts.Tutorial
{
    public class TutTimeListener : MonoBehaviour, ITutorialListener
    {
        public float TimeToNextPanel { get; set; }

        private CountdownTimer _timer;

        public void Invoke(Action callback)
        {
            _timer = new CountdownTimer(TimeToNextPanel);
            _timer.OnTimerEnd += callback;
            _timer.Start();
        }
    }
}
