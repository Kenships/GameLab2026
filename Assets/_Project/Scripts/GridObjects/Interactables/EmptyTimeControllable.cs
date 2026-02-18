using System;
using _Project.Scripts.GridObjects.Interface;
using _Project.Scripts.Interaction.Interface;
using _Project.Scripts.Util.Timer.Timers;
using Obvious.Soap;
using UnityEngine;

namespace _Project.Scripts.GridObjects.Interactables
{
    public class EmptyTimeControllable : PickupObjectBase, ITimeControllable
    {
        [Header("Time Control Settings")]
        [SerializeField] private FloatVariable time;
        [SerializeField] private float fastForwardSpeed;
        [SerializeField] private float rewindSpeed;
        [field: SerializeField] public bool IsWinding { get; set; }
        private float _windingSpeed;

        private void Start()
        {
            Decay();
        }

        public void FastForward()
        {
            IsWinding = true;
            _windingSpeed = fastForwardSpeed;
        }

        public void CancelFastForward()
        {
            IsWinding = false;
            Decay();
        }

        public void Rewind()
        {
            IsWinding = true;
            _windingSpeed = -rewindSpeed;
        }

        public void CancelRewind()
        {
            IsWinding = false;
            Decay();
        }

        private void Decay()
        {
            _windingSpeed = - 0.5f * rewindSpeed;
        }

        private void Update()
        {
            time.Value = Mathf.Clamp(time.Value + _windingSpeed * Time.deltaTime, 0f, 100f);
        }
    }
}
