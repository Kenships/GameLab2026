using System;
using UnityEngine;
using UnityEngine.Events;

namespace _Project.Scripts.Core.HealthManagement
{
    public class Health : MonoBehaviour
    {
        public UnityAction<float> OnHealthChanged;
        public UnityAction<int> OnStageChanged;
        public UnityAction OnDeath;
        public UnityAction OnFullHp;

        [field: SerializeField] public float CurrentHealth { get; private set; }
        [field: SerializeField] public float MaxHealth { get; set; }
        
        [SerializeField] float[] _healthStages;
        
        [SerializeField] public bool ScreenShakeEnabled = true; 
        [SerializeField] public bool HealthBarShakeEnabled = true;
        [SerializeField] public bool VHSShakeEnabled = true;
        [SerializeField] private ShakeEffect shakeEffectCamera;
        [SerializeField] private ShakeEffect shakeEffectVHS;
        [SerializeField] private ShakeEffect shakeEffectHealthBar;
        public float[] HealthStages => _healthStages;

        public void Initialize(float maxHealth, float initialHealth = -1f)
        {
            MaxHealth = maxHealth;
            CurrentHealth = initialHealth >= 0 ? initialHealth : MaxHealth;
        }

        public void Initialize(float maxHealth, float[] healthStages, float initialHealth = -1f)
        {
            this._healthStages = healthStages;
            Initialize(maxHealth, initialHealth);
        }

        public void AddToHealth(float change)
        {
            float healthDelta = Mathf.Min(change, MaxHealth - CurrentHealth);

            UpdateStage(healthDelta);

            CurrentHealth += healthDelta;

            OnHealthChanged?.Invoke(healthDelta);
            
            if (healthDelta < 0 ) // check if damage or healing
            {
                if (ScreenShakeEnabled &&  shakeEffectCamera != null) 
                {
                    shakeEffectCamera.Shake(1f);
                }
                if (HealthBarShakeEnabled &&   shakeEffectHealthBar != null) 
                {
                    shakeEffectHealthBar.Shake(1f);
                }
                if (VHSShakeEnabled &&   shakeEffectVHS != null) 
                {
                    shakeEffectVHS.Shake(1f);
                }
            }

            if (CurrentHealth <= 0)
            {
                OnDeath?.Invoke();
                CurrentHealth = 0;
            }

            if (CurrentHealth >= MaxHealth)
            {
                OnFullHp?.Invoke();
            }
        }

        private void UpdateStage(float healthDelta)
        {
            if (HealthStages == null || HealthStages.Length == 0)
            {
                return;
            }

            int currentStage = GetStageIndexFromHealth(CurrentHealth);
            int stage = GetStageIndexFromHealth(CurrentHealth + healthDelta);
            
            if (stage == currentStage)
            {
                return;
            }

            if (currentStage > stage)
            {
                (currentStage, stage) = (stage, currentStage);
            }

            //Invokes all state changes from current stage to the new stage
            for (int i = currentStage + 1; i <= stage; i++)
            {
                Debug.Log($"Stage changed to {i}");
                OnStageChanged?.Invoke(i);
            }
        }

        public int GetStageIndexFromHealth(float health)
        {
            for (int i = 0; i < HealthStages.Length; i++)
            {
                if (health < HealthStages[i])
                {
                    return i;
                }
            }

            return HealthStages.Length;
        }
    }
}
