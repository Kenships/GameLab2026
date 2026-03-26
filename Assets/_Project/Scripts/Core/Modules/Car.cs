using _Project.Scripts.Core.Enemies;
using _Project.Scripts.Core.HealthManagement;
using _Project.Scripts.Core.Modules.Base_Class;
using _Project.Scripts.Core.Player;
using _Project.Scripts.Effects.Inflictors;
using System.Collections.Generic;
using UnityEngine;
using PrimeTween;
using System.Globalization;

namespace _Project.Scripts.Core.Modules
{
    public class Car : Module
    {
        [SerializeField] private EnemyEffectInflictor inflictor;
        [SerializeField] private Transform destination;
        [SerializeField] private float fastForwardSpeed = 15f;
        [SerializeField] private float rewindSpeed = 10f;
        [SerializeField] private RangeDetector backDetector;
        [SerializeField] private RangeDetector frontDetector;
        [SerializeField] private GameObject newCarModel;
        [SerializeField] private GameObject crashedCarModel;


        private Vector3 _originalPosition;
        private Vector3 _endPosition;
        private bool _fastForwarding = false;
        private bool _rewinding = false;
        private List<EnemyBase> _enemies;
        private Tween _rewindTween;
        private Tween _fastForwardTween;

        void Start()
        {
            _enemies = new List<EnemyBase>();
            _originalPosition = transform.position;
            _endPosition = destination.position;
            backDetector.OnObjectEnter += OnEnter;
            frontDetector.OnObjectEnter += OnEnter;
        }

        protected override void LoadState()
        {
        
        }

        protected override void AttackState()
        {

        }

        protected override void UsedState()
        {

        }

        private void PeformAttack()
        {
            if (_rewinding && !_rewindTween.isAlive)
            {
                float distance = Vector3.Distance(transform.position, _endPosition);
                float duration = distance / rewindSpeed;

                _rewindTween = Tween.Position(transform, _endPosition, duration, ease: Ease.OutSine)
                    .OnUpdate(this, (target, tween) =>
                    {
                        target.frontDetector.GetObjectTypeInRangeNoAlloc(target._enemies);

                        if (tween.elapsedTime >= 0.05f)
                        {
                            target.crashedCarModel.SetActive(false);
                            target.newCarModel.SetActive(true);
                        }
                    })
                    .OnComplete(this, target => target._rewinding = false);
            }

            if (_fastForwarding && !_fastForwardTween.isAlive)
            {
                float distance = Vector3.Distance(transform.position, _originalPosition);
                float duration = distance / fastForwardSpeed;

                _fastForwardTween = Tween.Position(transform, _originalPosition, duration, ease: Ease.InSine)
                    .OnUpdate(this, (target, tween) =>
                    {
                        target.backDetector.GetObjectTypeInRangeNoAlloc(target._enemies);

                        if (tween.elapsedTime >= (duration - 0.05f))
                        {
                            target.crashedCarModel.SetActive(true);
                            target.newCarModel.SetActive(false);
                        }
                    })
                    .OnComplete(this, target => target._fastForwarding = false);
            }
        }

        protected override void OnStateChanged(ModuleState newState)
        {
            switch (state)
            {
                case ModuleState.Load:
                    break;
                case ModuleState.Attack:
                    frontDetector.ResetRangeDetection();
                    backDetector.ResetRangeDetection();
                    PeformAttack();
                    break;
                case ModuleState.Used:
                    break;
            }
        }

        public override void FastForward()
        {
            if (_rewinding)
            {
                return;
            }

            _fastForwarding = true;
            state = ModuleState.Attack;
        }

        public override void CancelFastForward()
        {
            _fastForwarding = false;
            state = ModuleState.Load;
        }

        public override void Rewind()
        {
            if (_fastForwarding)
            {
                return;
            }
        
            _rewinding = true;
            state = ModuleState.Attack;
        }

        public override void CancelRewind()
        {
            state = ModuleState.Load;
            _rewinding = false;
        }
    
    
        public override void ShowVisual(PlayerData.PlayerID playerID)
        {
        
        }

        public override void HideVisual(PlayerData.PlayerID playerID)
        {
      
        }

        private void OnEnter(Collider col)
        {
            Debug.Log(col.name);
            inflictor.Inflict(col.GetComponent<EnemyBase>());
        }
    }
}
