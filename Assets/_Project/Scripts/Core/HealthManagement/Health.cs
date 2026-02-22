using UnityEngine;
using UnityEngine.Events;

namespace _Project.Scripts.Core.HealthManagement
{
    public class Health : MonoBehaviour
    {
        public UnityAction<float> OnHealthChanged;
        public UnityAction OnDeath;
        
        [field: SerializeField] public float CurrentHealth { get; private set; }
        public float MaxHealth => _maxHealth;
        
        private float _maxHealth;
    

        public void Initialize(float maxHealth)
        {
            _maxHealth = maxHealth;
            CurrentHealth = _maxHealth;
        }

        public void AddToHealth(float change)
        {
            float healthDelta = Mathf.Min(change, _maxHealth - CurrentHealth);
            
            CurrentHealth += healthDelta;
            
            OnHealthChanged?.Invoke(healthDelta);

            if (CurrentHealth <= 0)
            {
                Kill();
            }
        }

        private void Kill()
        {
            OnDeath?.Invoke();
        }
    }
}

