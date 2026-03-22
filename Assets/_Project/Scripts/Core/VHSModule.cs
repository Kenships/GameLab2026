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
        [SerializeField] private float vhsMaxHealth = 300f;
        [SerializeField] private float defaultRewindSpeed = 1f;
        [SerializeField] private float fastForwardMultiplier = 1.2f;
        [Tooltip("Please keep the array in sorted ascending order")]
        [SerializeField] private float[] mileStones;
        
        private HashSet<int> _reachedMilestones = new();

        private List<IEffect<IDamageable>> _damageEffects = new();
        private Health _myHealth;
        private bool _isFastForwarding;
        private SceneLoader _sceneLoader;
        private IAudioPlayer currentFastForwardSound;



        protected override void OnAwake()
        {
            Location = transform;
            _myHealth = gameObject.GetComponent<Health>();
            _myHealth.Initialize(vhsMaxHealth, mileStones, 0);
            _sceneLoader = GetComponent<SceneLoader>();

            _myHealth.OnFullHp += HandleVHSFullHp;
        }

        private void Start()
        {
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
            _myHealth.OnFullHp -= HandleVHSFullHp;
        }

        private void HandleVHSFullHp()
        {
            vhsFullHPEvent.Raise();
        }

        private void MilestoneReached(int stage)
        {
            Debug.Log("Milestone reached");
            if (!_reachedMilestones.Add(stage))
            {
                return;
            }

            _audioPooler.StopAllSFX();

            _sceneLoader.LoadScene();
            Time.timeScale = 0f;
            PlayerInteractionController.isTimeFlowing = false;
        }
        public void Damage(float damage)
        {
            _myHealth.AddToHealth(-damage);
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
            float delta = _isFastForwarding ? defaultRewindSpeed * fastForwardMultiplier : defaultRewindSpeed;
            
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
            // NOP
        }

        public override void CancelRewind()
        {
            // NOP
        }
        
        public override void FastForward()
        {
            _isFastForwarding = true;

            if (currentFastForwardSound != null)
            {
                currentFastForwardSound.Stop();
                currentFastForwardSound = null;
            }

            currentFastForwardSound = _audioPooler.New2DAudio(fastForwardSound).OnChannel(AudioType.Sfx)
                .SetVolume(fastForwardSoundVolume).LoopAudio().Play();
        }

        public override void CancelFastForward()
        {   
            _isFastForwarding = false;
            currentFastForwardSound?.Stop();
            currentFastForwardSound = null;
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
