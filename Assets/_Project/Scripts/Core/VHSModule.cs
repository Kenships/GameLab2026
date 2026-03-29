using _Project.Scripts.Core.AudioPooling;
using _Project.Scripts.Core.AudioPooling.Interface;
using _Project.Scripts.Core.HealthManagement;
using _Project.Scripts.Core.Modules.Base_Class;
using _Project.Scripts.Core.Player;
using _Project.Scripts.Core.SceneLoading;
using _Project.Scripts.Effects.Interface;
using _Project.Scripts.Util.ExtensionMethods;
using Obvious.Soap;
using System.Collections.Generic;
using UnityEngine;
using AudioType = _Project.Scripts.Core.AudioPooling.Interface.AudioType;

namespace _Project.Scripts.Core
{
    
    public class VHSModule : Module, IDamageable
    {
        public static Transform Location;
        
        [Header("References")]
        [SerializeField] private GameObject player1Visual;
        [SerializeField] private GameObject player2Visual;
        [SerializeField] private ScriptableEventNoParam vhsFullHPEvent;
    
        [Header("VHS Settings")]
        [SerializeField] private bool enableOnAwake = true;
        [SerializeField] private float vhsMaxHealth = 300f;
        [SerializeField] private float rewindSpeed = 1f;
        [Tooltip("Please keep the array in sorted ascending order")]
        [SerializeField] private float[] mileStones;
        
        private HashSet<int> _reachedMilestones = new();

        private List<IEffect<IDamageable>> _damageEffects = new();
        private Health _myHealth;
        private bool _isRewinding;
        private SceneLoader _sceneLoader;
        private IAudioPlayer currentRewindSound;



        protected override void OnAwake()
        {
            EnableModule = enableOnAwake;
            Location = transform;
            _myHealth = gameObject.GetComponent<Health>();
            _myHealth.Initialize(vhsMaxHealth, mileStones, vhsMaxHealth);
            _sceneLoader = GetComponent<SceneLoader>();
            //_myHealth.OnDeath += HandleVHSFullHp;
        }

        private void Start()
        {
            GameManager.Instance.score = _myHealth.CurrentHealth;
            GameManager.Instance.RestartTimer();
            GameManager.Instance.StartTimer();
            _myHealth.OnStageChanged += MilestoneReached;
        }

        private void OnDestroy()
        {
            _myHealth.OnStageChanged -= MilestoneReached;
            foreach (var effect in _damageEffects)
            {
                effect.OnComplete -= RemoveEffect;
                effect.Cancel();
            }
            //_myHealth.OnFullHp -= HandleVHSFullHp;
        }

        private void HandleVHSFullHp()
        {
            vhsFullHPEvent.Raise();
        }

        private void MilestoneReached(int stage)
        {
            //Debug.Log("Milestone reached");
            //if (!_reachedMilestones.Add(stage)) return;

            //_audioPooler.StopAllSFX();
            //_sceneLoader.LoadScene();
            //Time.timeScale = 0f;
            //PlayerInteractionController.isTimeFlowing = false;
        }
        public void Damage(float damage)
        {
            _myHealth.AddToHealth(-damage);
            GameManager.Instance.score = _myHealth.CurrentHealth;
        }

        public void ApplyEffect(IEffect<IDamageable> effect)
        {
            effect.OnComplete += RemoveEffect;
            _damageEffects.Add(effect);
            effect.Apply(this);
        }

        public void RemoveEffect(IEffect<IDamageable> effect)
        {
            effect.OnComplete -= RemoveEffect;
            _damageEffects.Remove(effect);
        }

        protected override void LoadState()
        {
            float delta = _isRewinding ? rewindSpeed : 0;

            _myHealth.AddToHealth(delta * Time.deltaTime);
        }

        protected override void AttackState()
        {
            // NOP
        }

        protected override void UsedState()
        {
            // NOP
        }

        protected override void OnStateChanged(ModuleState prevState)
        {
            // NOP
        }

        public override void Rewind()
        {
            _isRewinding = true;

            if (currentRewindSound != null)
            {
                currentRewindSound.Stop();
                currentRewindSound = null;
            }

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

        }

        public override void CancelFastForward()
        {

        }

        public override void ShowVisual(PlayerData.PlayerID playerIndex)
        {
            if (playerIndex == PlayerData.PlayerID.Player1)
            {
                player1Visual.SetActive(true);
            }
            else
            {
                player2Visual.SetActive(true);
            }
        }

        public override void HideVisual(PlayerData.PlayerID playerIndex)
        {
            if (playerIndex == PlayerData.PlayerID.Player1)
            {
                player1Visual.SetActive(false);
            }
            else
            {
                player2Visual.SetActive(false);
            }
        }

        private void OnValidate()
        {
            #if UNITY_EDITOR
            
            _myHealth ??= gameObject.GetOrAdd<Health>();
            _myHealth.Initialize(vhsMaxHealth, mileStones, 0);
            
            #endif
        }
    }
}
