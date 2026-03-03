using System;
using _Project.Scripts.Util.ExtensionMethods;
using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Core.HealthManagement
{
    [RequireComponent(typeof(Slider))]
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private Health health;
        private Slider _slider;
        private RectTransform _rectTransform;
        private CanvasGroup _canvasGroup;

        private Vector2 _initialSizeDelta;
        public void Initialize(Health initHealth)
        {
            //health = initHealth;
        }

        private void Start()
        {
            _slider = GetComponent<Slider>();
            _rectTransform = GetComponent<RectTransform>();
            _canvasGroup = gameObject.GetOrAdd<CanvasGroup>();
            
            _initialSizeDelta = _rectTransform.sizeDelta;
            
            FindHealthComponent();
            // if (!health)
            // {
            //     Debug.LogError("Health is null");
            //     return;
            // }
            health.OnHealthChanged += UpdateHealthBar;
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
            // Debug.Log(healthDelta);

            // if (healthDelta > 0)
            // {
            //     Tween.UISizeDelta(
            //         target: _rectTransform,
            //         endValue: _initialSizeDelta * 2f,
            //         duration: 0.3f,
            //         ease: Ease.InOutExpo
            //     );
            // }
            // else
            // {
            //     Tween.UISizeDelta(
            //         target: _rectTransform,
            //         endValue: _initialSizeDelta,
            //         duration: 0.15f,
            //         ease: Ease.InOutExpo
            //     );
            // }

            if (health.MaxHealth == 0)
            {
                return;
            }
            _slider.value = health.CurrentHealth / health.MaxHealth;
        }
    }
}
