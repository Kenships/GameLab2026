using System.Collections.Generic;
using _Project.Scripts.Core.HealthManagement;
using _Project.Scripts.Core.Modules.Base_Class;
using _Project.Scripts.Core.Player;
using _Project.Scripts.Core.SceneLoading;
using _Project.Scripts.Effects;
using _Project.Scripts.Effects.Interface;
using _Project.Scripts.Targeting;
using _Project.Scripts.Util.ExtensionMethods;
using UnityEngine;

namespace _Project.Scripts.Core
{
    
    public class VHSModule : Module, IDamageable
    {
        public static Transform Location;
        
        [Header("References")]
        [SerializeField] private GameObject player1Visual;
        [SerializeField] private GameObject player2Visual;
    
        [Header("VHS Settings")]
        [SerializeField] private float vhsMaxHealth = 300f;
        [SerializeField] private float defaultRewindSpeed = 1f;
        [SerializeField] private float fastForwardMultiplier = 1.2f;

        private List<IEffect<IDamageable>> _damageEffects = new();
        private Health _myHealth;
        private bool _isFastForwarding;
        
        protected override void OnAwake()
        {
            _myHealth = gameObject.GetOrAdd<Health>();
            _myHealth.Initialize(vhsMaxHealth, 0);
            
            // TODO: Temporary please fix
            _myHealth.OnFullHp += () => GetComponent<SceneLoader>().LoadScene();
        }

        private void OnDestroy()
        {
            foreach (var effect in _damageEffects)
            {
                effect.OnComplete -= RemoveEffect;
                effect.Cancel();
            }
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
        }

        public override void CancelFastForward()
        {   
            _isFastForwarding = false;
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
    }
}
