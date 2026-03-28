using _Project.Scripts.Core.Enemies;
using _Project.Scripts.Core.Modules.Base_Class;
using _Project.Scripts.Core.Player;
using _Project.Scripts.Effects.Inflictors;
using System.Collections.Generic;
using UnityEngine;
using PrimeTween;

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

        protected virtual void PerformAttack(bool fastForward)
        {
            if (!fastForward && !_rewindTween.isAlive)
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
                    .OnComplete(() => state = ModuleState.Load);
            }

            if (fastForward && !_fastForwardTween.isAlive)
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
                    .OnComplete(() => state = ModuleState.Used);
            }
        }

        protected override void OnStateChanged(ModuleState newState)
        {
            
        }

        public override void FastForward(PlayerData.PlayerID playerID)
        {
            base.FastForward(playerID);
            frontDetector.ResetRangeDetection();
            backDetector.ResetRangeDetection();
            PerformAttack(true);
            CancelFastForward(playerID);
        }

        public override void Rewind(PlayerData.PlayerID playerID)
        {
            base.Rewind(playerID);
            frontDetector.ResetRangeDetection();
            backDetector.ResetRangeDetection();
            PerformAttack(false);
            CancelRewind(playerID);
        }

        public override void ShowVisual(PlayerData.PlayerID playerID)
        {
        
        }

        public override void HideVisual(PlayerData.PlayerID playerID)
        {
      
        }

        private void OnEnter(Collider col)
        {
            inflictor.Inflict(col.GetComponent<EnemyBase>());
        }
    }
}
