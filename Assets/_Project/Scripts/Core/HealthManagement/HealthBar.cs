using System;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Core.HealthManagement
{
    [RequireComponent(typeof(Slider))]
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private Health health;
        private Slider _slider;
        
        
        public void Initialize(Health initHealth)
        {
            health = initHealth;
        }

        private void Awake()
        {
            _slider = GetComponent<Slider>();
        }

        private void Start()
        {
            FindHealthComponent();
            if (!health)
            {
                Debug.LogError("Health is null");
                return;
            }
            health.OnHealthChanged += UpdateHealthBar;
            _slider.value = health.CurrentHealth / health.MaxHealth;
        }

        private void FindHealthComponent()
        {
            health ??= GetComponent<Health>();
            health ??= GetComponentInParent<Health>();
            health ??= GetComponentInChildren<Health>();
        }

        // Health delta can be used to show animation or visual effects
        private void UpdateHealthBar(float healthDelta)
        {
            // Temp logic
            Debug.Log(healthDelta);
            
            _slider.value = health.CurrentHealth / health.MaxHealth;
        }
    }
}
