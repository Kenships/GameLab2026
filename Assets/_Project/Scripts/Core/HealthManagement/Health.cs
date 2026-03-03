using UnityEngine;
using UnityEngine.Events;

namespace _Project.Scripts.Core.HealthManagement
{
    public class Health : MonoBehaviour
    {
        public UnityAction<float> OnHealthChanged;
        public UnityAction OnDeath;
        public UnityAction OnFullHp;
        
        [field: SerializeField] public float CurrentHealth { get; private set; }
        public float MaxHealth => _maxHealth;
        
        private float _maxHealth;
    

        public void Initialize(float maxHealth, float initialHealth = -1f)
        {
            _maxHealth = maxHealth;
            CurrentHealth = initialHealth >= 0 ? initialHealth : _maxHealth;
        }

        public void AddToHealth(float change)
        {
            float healthDelta = Mathf.Min(change, _maxHealth - CurrentHealth);
            
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
    }
}

