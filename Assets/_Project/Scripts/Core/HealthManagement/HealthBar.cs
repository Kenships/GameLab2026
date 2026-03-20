using System;
using System.Collections.Generic;
using Unity.VisualScripting;
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
        [SerializeField] private List<RectTransform> stageDeviderBars;

        private void Start()
        {
            _slider = GetComponent<Slider>();
            _rectTransform = GetComponent<RectTransform>();
            
            FindHealthComponent();
            if (!health)
            {
                Debug.LogError("Health is null");
                return;
            }
            health.OnHealthChanged += UpdateHealthBar;
            
            _slider.value = health.CurrentHealth / health.MaxHealth;
            
            RebuildDeviderBars();
        }

        private void FindHealthComponent()
        {
            health ??= GetComponent<Health>();
            health ??= GetComponentInParent<Health>();
            health ??= GetComponentInChildren<Health>();
        }

        private void Update()
        {
            if (stageDeviderBars.Count == 0)
            {
                return;
            }
            
            // Assume that healthstages does not change on runtime
            int healthIndex = health.GetStageIndexFromHealth(health.CurrentHealth);
            
            for (int i = 0; i < healthIndex; i++)
            {
                stageDeviderBars[i].gameObject.SetActive(false);
            }
        }

        // Health delta can be used to show animation or visual effects
        private void UpdateHealthBar(float healthDelta)
        {
            if (health.MaxHealth == 0)
            {
                return;
            }
            _slider.value = health.CurrentHealth / health.MaxHealth;
        }

        [ContextMenu("Rebuild Devider Bars")]
        private void RebuildDeviderBars()
        {
            foreach (var stageDeviderBar in stageDeviderBars)
            {
                if (!stageDeviderBar){ continue;}
                
                DestroyImmediate(stageDeviderBar.gameObject);
            }
            stageDeviderBars.Clear();
            
            FindHealthComponent();
            _rectTransform = GetComponent<RectTransform>();
            
            if (!health || health.HealthStages == null)
            {
                return;
            }
            
            foreach (float healthStage in health.HealthStages)
            {
                if (healthStage <= 0)
                {
                    continue;
                }
                if (healthStage > health.MaxHealth)
                {
                    continue;
                }
                
                var stageDeviderBar = new GameObject("StageDeviderBar").AddComponent<RectTransform>();
                stageDeviderBar.AddComponent<Image>();
                stageDeviderBar.sizeDelta = new Vector2(_rectTransform.rect.width / 250f, _rectTransform.rect.height * 1.5f);
                stageDeviderBar.SetParent(transform);
                stageDeviderBar.pivot = new Vector2(0, 0.5f);
                stageDeviderBar.anchoredPosition = new Vector2(healthStage / health.MaxHealth * _rectTransform.rect.width - _rectTransform.rect.width / 2f, 0);
                
                var visual = stageDeviderBar.GetComponent<Image>();
                visual.color = Color.white;
                visual.raycastTarget = false;
                
                stageDeviderBars.Add(stageDeviderBar);
            }
        }
    }
}
