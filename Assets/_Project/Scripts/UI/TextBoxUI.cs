using System;
using _Project.Scripts.Util.Timer.Timers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Project.Scripts.UI
{
    public class TextBoxUI : MonoBehaviour
    {
        private StopMotionUI stopMotionUI;

        private CountdownTimer _timer;
        private void Awake()
        {
            _timer = new CountdownTimer(Random.Range(0.5f, 2f));
            stopMotionUI = GetComponent<StopMotionUI>();
            stopMotionUI.destroyOnEnd = false;
            
            _timer.OnTimerEnd += OnTimerEnd;
            _timer.Start();
        }

        private void OnTimerEnd()
        {
            stopMotionUI.Play();
            _timer.Reset(Random.Range(0.5f, 2f));
        }
    }
}
