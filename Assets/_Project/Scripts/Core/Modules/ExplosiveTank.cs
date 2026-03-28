using System.Collections.Generic;
using _Project.Scripts.Core.HealthManagement;
using _Project.Scripts.Core.Modules.Base_Class;
using _Project.Scripts.Core.Player;
using _Project.Scripts.Effects.Interface;
using UnityEngine;

namespace _Project.Scripts.Core.Modules
{
    public class ExplosiveTank : HpModuleBase, IDamageable
    {
        [Header("References")]
        [SerializeField] private ParticleSystem explosionParticle;
        [SerializeField] private GameObject loadModel;
        [SerializeField] private GameObject usedModel;

        [Header("Explosive Tank Settings")]
        [SerializeField] private float damage = 90f;
        [Tooltip("Does damage twice, to fit the explosion VFX")]
        [SerializeField] private float timeGapBeforeSecondAttack = 0.75f;

        [Header("Player Selection Visuals")]
        [SerializeField] private GameObject player1Visual;
        [SerializeField] private GameObject player2Visual;
        private RangeDetector _rangeDetector; // rangeType is circle
        private List<IDamageable> _enemies;

        protected override void OnAwake()
        {
            base.OnAwake();
            _rangeDetector = GetComponent<RangeDetector>();
            _enemies = new List<IDamageable>();
            
            loadModel.SetActive(false);
            usedModel.SetActive(true);

            state = ModuleState.Used;
        }

        protected override void Start()
        {
            base.Start();
            var main = explosionParticle.main;
            main.stopAction = ParticleSystemStopAction.Callback;
        
        
            if (!_rangeDetector)
            {
                Debug.Log("missing rangeDetector");
                return;
            }

            if (!explosionParticle)
            {
                Debug.Log("missing particle");
            }
        }

        private void PerformAttack()
        {
            _rangeDetector.GetObjectTypeInRangeNoAlloc(_enemies);

            if (_enemies.Count <= 0)
            {
                return;
            }

            foreach (IDamageable enemy in _enemies)
            {
                enemy?.Damage(damage);
            }
        }

        public override void ShowVisual(PlayerData.PlayerID playerID)
        {
            if (!player1Visual || !player2Visual)
            {
                Debug.LogWarning("Player Selection Visuals not set");
                return;
            }

            if (playerID == PlayerData.PlayerID.Player1)
            {
                player1Visual.SetActive(true);
            }
            else
            {
                player2Visual.SetActive(true);
            }
        }

        public override void HideVisual(PlayerData.PlayerID playerID)
        {
            if (!player1Visual || !player2Visual)
            {
                Debug.LogWarning("Player Selection Visuals not set");
                return;
            }

            if (playerID == PlayerData.PlayerID.Player1)
            {
                player1Visual.SetActive(false);
            }
            else
            {
                player2Visual.SetActive(false);
            }
        }

        #region State Methods

        protected override void LoadState()
        {
            if (_isFastForwarding)
            {
                state = ModuleState.Attack;
            }
        }

        protected override void AttackState()
        {
        
        }

        protected override void UsedState()
        {
            if (_isRewinding)
            {
                _health.AddToHealth(defaultRecoverySpeed * Time.deltaTime);
            }

            if (_currentFastForwardSound != null)
            {
                _currentFastForwardSound.Stop();
                _currentFastForwardSound = null;
            }
        }

        protected override void OnStateChanged(ModuleState prevState)
        {
            switch (state)
            {
                case ModuleState.Load:
                    usedModel.SetActive(false);
                    loadModel.SetActive(true);
                    break;
                case ModuleState.Attack:
                    PerformAttack();
                    explosionParticle.Play();
                    Invoke(nameof(PerformAttack), timeGapBeforeSecondAttack);
                    loadModel.SetActive(false);
                    usedModel.SetActive(true);
                    _health.AddToHealth(int.MinValue);
                    break;
                case ModuleState.Used:
                    break;
            }
        }

        #endregion

        public void Damage(float damage)
        {
            _health.AddToHealth(-damage);
        }

        public void ApplyEffect(IEffect<IDamageable> effect)
        {
            
        }

        public void RemoveEffect(IEffect<IDamageable> effect)
        {
            
        }
    }
}
