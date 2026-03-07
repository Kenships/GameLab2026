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

        [field: SerializeField]
        public float CurrentHealth { get; private set; }
        [field: SerializeField]
        public float MaxHealth { get; set; }

        [Tooltip("Please keep the array in sorted ascending order")]
        [SerializeField] 
        private float[] healthStages;
        
        public float[] HealthStages => healthStages;

        public void Initialize(float maxHealth, float initialHealth = -1f)
        {
            MaxHealth = maxHealth;
            CurrentHealth = initialHealth >= 0 ? initialHealth : MaxHealth;
        }

        public void Initialize(float maxHealth, float[] healthStages, float initialHealth = -1f)
        {
            this.healthStages = healthStages;
            Initialize(maxHealth, initialHealth);
        }

        public void AddToHealth(float change)
        {
            float healthDelta = Mathf.Min(change, MaxHealth - CurrentHealth);

            UpdateStage(healthDelta);

            CurrentHealth += healthDelta;

            OnHealthChanged?.Invoke(healthDelta);

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
            if (HealthStages is not { Length: > 0 })
            {
                return;
            }

            int currentStage = GetStageIndexFromHealth(CurrentHealth);
            int stage = GetStageIndexFromHealth(CurrentHealth + healthDelta);

            if (stage == currentStage)
            {
                return;
            }

            if (stage > currentStage)
            {
                (currentStage, stage) = (stage, currentStage);
            }

            //Invokes all state changes from current stage to the new stage
            for (int i = currentStage + 1; i <= stage; i++)
            {
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

            return HealthStages.Length - 1;
        }
    }
}
