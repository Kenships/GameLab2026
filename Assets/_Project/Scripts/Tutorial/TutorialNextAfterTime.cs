using System;
using _Project.Scripts.Util.Timer.Timers;
using UnityEngine;

namespace _Project.Scripts.Tutorial
{
    public class TutorialNextAfterTime : MonoBehaviour, ITutorialListener
    {
        [SerializeField] private float timeToNextPanel;

        private CountdownTimer _timer;

        public void Invoke(Action callback)
        {
            _timer = new CountdownTimer(timeToNextPanel);
            _timer.OnTimerEnd += callback;
        }
    }
}
