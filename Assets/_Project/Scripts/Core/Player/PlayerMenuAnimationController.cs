using System;
using _Project.Scripts.Util.Timer.Timers;
using UnityEngine;
using Random = System.Random;

namespace _Project.Scripts.Core.Player
{
    public class PlayerMenuAnimationController : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private float transitionSpeed = 5f;
        [SerializeField, Range(0f, 1f)] private float scratchChance = 0.2f;

        private CountdownTimer _countdownTimer;

        private float _targetHappiness;
        private float _currentHappiness;
        private Random _rng;
    
        private void Start()
        {
            _rng = new Random(Guid.NewGuid().GetHashCode());
            
            _countdownTimer = new CountdownTimer(NextFloat(2f, 4f));
            _countdownTimer.OnTimerEnd += TimerEnd;
            _targetHappiness = NextFloat();
            _countdownTimer.Start();
        }

        private void OnDestroy()
        {
            _countdownTimer.OnTimerEnd -= TimerEnd;
            _countdownTimer.Stop();
            _countdownTimer = null;
        }

        private void TimerEnd()
        {
            _countdownTimer.Reset(NextFloat(2f, 4f));
            _targetHappiness = NextFloat();
        }

        private void FixedUpdate()
        {
            if (Mathf.Approximately(_currentHappiness, _targetHappiness))
            {
                return;
            }

            if (NextFloat() < scratchChance)
            {
                animator.SetTrigger("Scratch");
            }
            
            _currentHappiness = Mathf.Lerp(_currentHappiness, _targetHappiness, Time.deltaTime * transitionSpeed );
            animator.SetFloat("Happiness", _currentHappiness);
        }
        private float NextFloat(float min = 0f, float max = 1f)
        {
            return (float) _rng.NextDouble() * (max - min) + min;
        }

        public void Select()
        {
            animator.SetTrigger("Selected");
        }
    }
}
