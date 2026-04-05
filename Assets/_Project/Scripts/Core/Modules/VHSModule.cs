using System;
using System.Collections.Generic;
using _Project.Scripts.Core.HealthManagement;
using _Project.Scripts.Core.Modules.Base_Class;
using _Project.Scripts.Core.Player;
using _Project.Scripts.Core.SceneLoading;
using _Project.Scripts.Effects.Interface;
using _Project.Scripts.Util.ExtensionMethods;
using Obvious.Soap;
using UnityEngine;

namespace _Project.Scripts.Core.Modules
{
    
    public class VHSModule : Module, IDamageable
    {
        public static Transform Location;
        
        [Header("References")]
        [SerializeField] private GameObject player1Visual;
        [SerializeField] private GameObject player2Visual;
        [SerializeField] private ScriptableEventNoParam vhsDeathEvent;
    
        [Header("VHS Settings")]
        [SerializeField] private bool enableOnAwake = true;
        [SerializeField] private float vhsMaxHealth = 300f;
        [SerializeField] private float rewindSpeed = 1f;
        [Tooltip("Please keep the array in sorted ascending order")]
        [SerializeField] private float[] mileStones;

        [Header("Damage Reduction Settings")]
        [SerializeField] private bool useDamageReductionThreshold = true;
        [SerializeField] private float damageReductionHealthThreshold = 100f;
        [SerializeField] [Range(0f, 1f)] private float damageMultiplierBelowThreshold = 0.5f;
        
        private HashSet<int> _reachedMilestones = new();

        private List<IEffect<IDamageable>> _damageEffects = new();
        private Health _myHealth;
        private SceneLoader _sceneLoader;



        protected override void OnAwake()
        {
            EnableModule = enableOnAwake;
            Location = transform;
            _myHealth = gameObject.GetComponent<Health>();
            _myHealth.Initialize(vhsMaxHealth, mileStones, vhsMaxHealth);
            _sceneLoader = GetComponent<SceneLoader>();
            _myHealth.OnDeath += HandleVHSDeath;
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
            _myHealth.OnDeath -= HandleVHSDeath;
        }

        private void HandleVHSDeath()
        {
            vhsDeathEvent.Raise();
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
            float finalDamage = damage;
            if (useDamageReductionThreshold && _myHealth.CurrentHealth <= damageReductionHealthThreshold)
            {finalDamage *= damageMultiplierBelowThreshold;}
            _myHealth.AddToHealth(-finalDamage);
            GameManager.Instance.score = _myHealth.CurrentHealth;
        }

        public void ApplyEffect<T>(IEffect<T> effect) where T : IDamageable
        {
            if (effect is not IEffect<IDamageable> damageEffect)
            {
                return;
            }
            
            damageEffect.OnComplete += RemoveEffect;
            _damageEffects.Add(damageEffect);
            damageEffect.Apply(this);
        }

        public void RemoveEffect(Guid effectID)
        {
            foreach (var effect in _damageEffects)
            {
                if (effect.InstanceID == effectID)
                {
                    effect.OnComplete -= RemoveEffect;
                    _damageEffects.Remove(effect);
                    return;
                }
            }
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
