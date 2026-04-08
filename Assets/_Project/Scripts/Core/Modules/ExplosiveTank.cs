using System;
using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Core.HealthManagement;
using _Project.Scripts.Core.InputManagement;
using _Project.Scripts.Core.Modules.Base_Class;
using _Project.Scripts.Core.Player;
using _Project.Scripts.Effects.Interface;
using _Project.Scripts.Effects.Runtime;
using _Project.Scripts.Util.Timer.Timers;
using Obvious.Soap;
using UnityEngine;
using UnityEngine.InputSystem;
using AudioType = _Project.Scripts.Core.AudioPooling.Interface.AudioType;

namespace _Project.Scripts.Core.Modules
{
    public class ExplosiveTank : PickupModuleBase, IDamageable
    {
        [Header("References")]
        [SerializeField] private ParticleSystem explosionParticle;
        [SerializeField] private GameObject loadModel;
        [SerializeField] private GameObject usedModel;
        [SerializeField] private GameObject fullTimeUI;
        [SerializeField] private FloatVariable hapticsStrength;

        [Header("Explosive Tank Settings")]
        [SerializeField] private float damage = 90f;
        [Tooltip("Does damage twice, to fit the explosion VFX")]
        [SerializeField] private float timeGapBeforeSecondAttack = 0.75f;

        [SerializeField] private float explosionDelay = 1f;


        [Header("Audio")][SerializeField] private AudioClip shootSound;
        [SerializeField] private float shootSoundVolume = 0.1f;

        [Header("Player Selection Visuals")]
        [SerializeField] private GameObject player1Visual;
        [SerializeField] private GameObject player2Visual;
        private RangeDetector _rangeDetector; // rangeType is circle
        private List<IDamageable> _enemies;
        private List<NESActionReader> _players;


        private CountdownTimer _delay;

        protected override void OnAwake()
        {
            base.OnAwake();
            _rangeDetector = GetComponent<RangeDetector>();
            _players = new List<NESActionReader>();
            _enemies = new List<IDamageable>();
            
            loadModel.SetActive(false);
            usedModel.SetActive(true);

            state = ModuleState.Used;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _delay.OnTimerEnd -= Attack;
            _delay = null;
        }

        protected override void Start()
        {
            base.Start();
            _delay = new CountdownTimer(explosionDelay);
            _delay.OnTimerEnd += Attack;

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
            _rangeDetector.GetObjectTypeInRangeNoAlloc(_players);
            _rangeDetector.GetObjectTypeInRangeNoAlloc(_enemies);

            foreach (var player in _players)
            {
                Debug.Log(player);
                if (player.TryGetGamePad(out Gamepad gamepad))
                {
                    StartCoroutine(ExplosionRumble(gamepad));
                }
            }
            
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
                    loadModel.SetActive(true);
                    usedModel.SetActive(false);
                    fullTimeUI.SetActive(true);
                    break;
                case ModuleState.Attack:
                    _audioPooler.New2DAudio(shootSound).OnChannel(AudioType.Sfx).SetVolume(shootSoundVolume).Play();
                    _delay.Reset(explosionDelay);

                    break;
                case ModuleState.Used:
                    usedModel.SetActive(true);
                    if (allowBrokenEffect) brokenEffect.Play();
                    allowBrokenEffect = true;
                    loadModel.SetActive(false);
                    fullTimeUI.SetActive(false);
                    break;
            }
        }
        private void Attack()
        {
            PerformAttack();
            explosionParticle.Play();
            Invoke(nameof(PerformAttack), timeGapBeforeSecondAttack);
            loadModel.SetActive(false);
            usedModel.SetActive(true);
            fullTimeUI.SetActive(false);
            _health.AddToHealth(int.MinValue);
        }



        #endregion

        public void Damage(float damage)
        {
            _health.AddToHealth(-damage);
        }

        public void ApplyEffect<T>(IEffect<T> effect) where T : IDamageable
        {
            
        }

        public void RemoveEffect(Guid id)
        {
            
        }

        private IEnumerator ExplosionRumble(Gamepad gamepad)
        {
            gamepad.SetMotorSpeeds(hapticsStrength.Value, hapticsStrength.Value);

            yield return new WaitForSecondsRealtime(0.3f);
            
            gamepad.SetMotorSpeeds(0f, 0f);
        }
    }
}
