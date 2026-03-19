using System.Collections.Generic;
using _Project.Scripts.Core.AudioPooling.Interface;
using _Project.Scripts.Core.HealthManagement;
using _Project.Scripts.Core.Modules.Base_Class;
using _Project.Scripts.Core.Player;
using _Project.Scripts.Effects.Interface;
using UnityEngine;
using AudioType = _Project.Scripts.Core.AudioPooling.Interface.AudioType;

namespace _Project.Scripts.Core.Modules
{
    public class ExplosiveTank : Module, IDamageable
    {
        [Header("References")]
        [SerializeField] private ParticleSystem explosionParticle;
        [SerializeField] private GameObject loadModel;
        [SerializeField] private GameObject usedModel;
    
        [Header("Time Settings")]
        [SerializeField] protected float maxHealth = 100f;
        [SerializeField] protected float defaultRecoverySpeed = 1f;

        [Header("Explosive Tank Settings")]
        [SerializeField] private float damage = 90f;
        [Tooltip("Does damage twice, to fit the explosion VFX")]
        [SerializeField] private float timeGapBeforeSecondAttack = 0.75f;

        [Header("Player Selection Visuals")]
        [SerializeField] private GameObject player1Visual;
        [SerializeField] private GameObject player2Visual;

        private Health _health;
        private RangeDetector _rangeDetector; // rangeType is circle
        private List<IDamageable> _enemies;
        
        private IAudioPlayer currentFastForwardSound;
        private IAudioPlayer currentRewindSound;

        private bool _isRewinding, _isFastForwarding;

        protected override void OnAwake()
        {
            _health = GetComponent<Health>();
            _rangeDetector = GetComponent<RangeDetector>();
            _health.Initialize(maxHealth, 0);
            _health.OnDeath += OnDeath;
            _health.OnFullHp += OnFullHP;
            _enemies = new List<IDamageable>();
            
            loadModel.SetActive(false);
            usedModel.SetActive(true);

            state = ModuleState.Used;
        }

        private void OnDeath()
        {
            state = ModuleState.Used;
        }

        private void OnFullHP()
        {
            state = ModuleState.Load;
        }

        protected void Start()
        {
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

            if (currentFastForwardSound != null)
            {
                currentFastForwardSound.Stop();
                currentFastForwardSound = null;
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

        public override void Rewind()
        {
            _isRewinding = true;
            currentRewindSound = _audioPooler.New2DAudio(rewindSound).OnChannel(AudioType.Sfx)
                .SetVolume(rewindSoundVolume).LoopAudio().Play();
        }

        public override void CancelRewind()
        {
            _isRewinding = false;
            currentRewindSound?.Stop();
            currentRewindSound = null;
        }

        public override void FastForward()
        {
            _isFastForwarding = true;
            currentFastForwardSound = _audioPooler.New2DAudio(fastForwardSound).OnChannel(AudioType.Sfx)
                .SetVolume(fastForwardSoundVolume).LoopAudio().Play();
        }
    
        public override void CancelFastForward()
        {
            _isFastForwarding = false;
            currentFastForwardSound?.Stop();
            currentFastForwardSound = null;
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
