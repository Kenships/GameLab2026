using _Project.Scripts.Core.HealthManagement;
using _Project.Scripts.Core.Modules.Base_Class;
using _Project.Scripts.Core.Player;
using _Project.Scripts.Effects.Interface;
using _Project.Scripts.Util.Timer.Timers;
using PrimeTween;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudioType = _Project.Scripts.Core.AudioPooling.Interface.AudioType;
using static Obvious.Soap.ScriptableSaveBase;

namespace _Project.Scripts.Core.Modules
{
    public class LazerCannon : PickupModuleBase, IDamageable
    {
        [Header("References")]
        [SerializeField] private Transform lazerBeamStartPos;
        [SerializeField] private GameObject lazerCannonNew;
        [SerializeField] private GameObject lazerCannonBroken;
        [SerializeField] private GameObject fullTimeUI;

        [Header("Lazer Beam Settings")]
        [SerializeField] private float damage = 90f;
        [SerializeField] private float lazerBeamDuration = 1.4f;
        [SerializeField] private float dps = 2f;

        [Header("Player Selection Visuals")]
        [SerializeField] private GameObject player1Visual;
        [SerializeField] private GameObject player2Visual;

        [Header("Audio")][SerializeField] private AudioClip shootSound;
        [SerializeField] private float shootSoundVolume = 0.1f;

        private RangeDetector _rangeDetector; // rangeType is rectangle
        private Transform _lazerBeam;
        private List<IDamageable> _enemies;
        private CountdownTimer _beamDurationTimer;
        private Coroutine _attackRoutine;

        protected override void Start()
        {
            _enemies = new List<IDamageable>();
            _beamDurationTimer = new CountdownTimer(lazerBeamDuration);
            _rangeDetector = GetComponent<RangeDetector>();
            
            
            _lazerBeam = lazerBeamStartPos.transform.GetChild(0);
            
            if (!_rangeDetector)
            {
                Debug.Log("missing rangeDetector");
                return;
            }
            
            base.Start();
        }

        private void PlayLazerBeamAnim()
        {
            float beamScale = _rangeDetector.length / (_lazerBeam.localScale.y * 2);
            Sequence.Create()
                .Chain(Tween.Scale(lazerBeamStartPos.transform, startValue: 0f, endValue: beamScale, duration: 0.2f, ease: Ease.OutExpo))
                .ChainDelay(lazerBeamDuration - 0.4f)
                .Chain(Tween.Scale(lazerBeamStartPos.transform, endValue: 0f, duration: 0.2f, ease: Ease.InExpo))
                .ChainCallback(() => {
                    lazerCannonNew.SetActive(false);
                    lazerCannonBroken.SetActive(true);
                });
        }

        private void PerformAttack()
        {
            _audioPooler.New2DAudio(shootSound).OnChannel(AudioType.Sfx).SetVolume(shootSoundVolume).RandomizePitch().Play();

            if (_beamDurationTimer.IsFinished)
            {
                return;
            }

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

        private IEnumerator AttackRoutine()
        {
            float attackInterval = 1f / dps;

            while (!_beamDurationTimer.IsFinished)
            {
                PerformAttack();
                yield return new WaitForSeconds(attackInterval);
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
            base.LoadState();
            if (_isFastForwarding)
            {
                state = ModuleState.Attack;
            }
        }

        protected override void OnStateChanged(ModuleState prevState)
        {
            switch (state)
            {
                case ModuleState.Load:
                    lazerCannonNew.SetActive(true);
                    lazerCannonBroken.SetActive(false);
                    fullTimeUI.SetActive(true);
                    break;
                case ModuleState.Attack:
                    _beamDurationTimer.Reset(lazerBeamDuration);
                    if (_attackRoutine != null)
                        StopCoroutine(_attackRoutine);
                    _attackRoutine = StartCoroutine(AttackRoutine());
                    PlayLazerBeamAnim();
                    _health.AddToHealth(int.MinValue);
                    fullTimeUI.SetActive(false);
                    break;
                case ModuleState.Used:
                    lazerCannonBroken.SetActive(true);
                    if (allowBrokenEffect) brokenEffect.Play();
                    allowBrokenEffect = true;
                    lazerCannonNew.SetActive(false);
                    fullTimeUI.SetActive(false);
                    if (_attackRoutine != null)
                    {
                        StopCoroutine(_attackRoutine);
                        _attackRoutine = null;
                    }
                    break;
            }
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

        private void OnDisable()
        {
            if (_attackRoutine != null)
                StopCoroutine(_attackRoutine);
        }
    }
}
