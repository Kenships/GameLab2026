using System;
using _Project.Scripts.Util.Timer.Timers;
using Obvious.Soap;
using UnityEngine;

namespace _Project.Scripts.Tutorial
{
    public class TutorialNextAfterTime : MonoBehaviour
    {
        [SerializeField] private float timeToNextPanel;
        [SerializeField] private ScriptableEventNoParam next;

        private CountdownTimer _timer;

        private void OnEnable()
        {
            _timer = new CountdownTimer(timeToNextPanel);
            _timer.Start();
            _timer.OnTimerEnd += TimerEnd;
        }

        private void TimerEnd()
        {
            next?.Raise();
        }

        private void OnDisable()
        {
            _timer.OnTimerEnd -= TimerEnd;
            _timer.Stop();
        }
    }
}
