using System;
using _Project.Scripts.Core;
using _Project.Scripts.Core.HealthManagement;
using _Project.Scripts.Core.Modules;
using Obvious.Soap;
using PrimeTween;
using UnityEngine;

namespace _Project.Scripts.Tutorial
{
    public class TutVHSListener : MonoBehaviour, ITutorialListener
    {
        [SerializeField] private VHSModule vhsModule;
        [SerializeField] private Light vhsLight;
        
        private Action _callback;
        
        private Health _health;
        
        private float _initialLightIntensity;
        
        private void Awake()
        {
            _health = vhsModule.GetComponent<Health>();
        }

        private void OnDestroy()
        {
            _health.OnFullHp -= VhsHPFullOnRaised;
            Tween.StopAll(vhsLight);
        }

        public void Invoke(Action callback)
        {
            _callback = callback;
            _initialLightIntensity = vhsLight.intensity;

            PulseLight();
            
            _health.OnFullHp += VhsHPFullOnRaised;
        }

        private void PulseLight()
        {
            Tween.LightIntensity(
                target: vhsLight,
                startValue: _initialLightIntensity,
                endValue: 15f,
                duration: 1f,
                ease: Ease.InOutQuad,
                cycleMode: CycleMode.Yoyo)
                .SetRemainingCycles(-1);
        }

        private void VhsHPFullOnRaised()
        {
            Tween.StopAll(vhsLight);
            vhsLight.intensity = _initialLightIntensity;
            _health.OnFullHp -= VhsHPFullOnRaised;
            _callback?.Invoke();
        }
    }
}
