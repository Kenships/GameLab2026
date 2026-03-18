using System.Collections.Generic;
using _Project.Scripts.Core.Enemies;
using _Project.Scripts.Core.Modules.Base_Class;
using _Project.Scripts.Core.Player;
using _Project.Scripts.Effects.Inflictors;
using UnityEngine;

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
    
    
        private Vector3 _originalPosition;
        private Vector3 _endPosition;
        private bool _fastForwarding = false;
        private bool _rewinding = false;
        private List<EnemyBase> _enemies;
    
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
            if (_fastForwarding)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    _endPosition,
                    rewindSpeed * Time.deltaTime
                );
                frontDetector.GetObjectTypeInRangeNoAlloc(_enemies);
            }

            if (_rewinding)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    _originalPosition,
                    fastForwardSpeed * Time.deltaTime
                );
                backDetector.GetObjectTypeInRangeNoAlloc(_enemies);
            }
        
        }

        protected override void UsedState()
        {

        }

        protected override void OnStateChanged(ModuleState newState)
        {
            if (state == ModuleState.Attack)
            {
                frontDetector.ResetRangeDetection();
                backDetector.ResetRangeDetection();
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
