using PrimeTween;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Tutorial
{
    public class TutCarListener : MonoBehaviour, ITutorialListener
    {
        [SerializeField] private TutCar tutCar;
        [SerializeField] private Light carLight;
        [SerializeField] private HintUI hintUI;
        [SerializeField] private RawImage arrow;

        private Action _callback;
        private float _initialLightIntensity;
        
        public void Invoke(Action callback)
        {
            _callback = callback;
            _initialLightIntensity = carLight.intensity;
            PulseCarLight();
            tutCar.OnCarAttack += OnCarAttack;
        }

        private void OnDestroy()
        {
            tutCar.OnCarAttack -= OnCarAttack;
            Tween.StopAll(carLight);
        }

        private void PulseCarLight()
        {
            Tween.LightIntensity(
                target: carLight,
                startValue: _initialLightIntensity,
                endValue: 10f,
                duration: 1f,
                ease: Ease.InOutQuad,
                cycleMode: CycleMode.Yoyo)
                .SetRemainingCycles(-1);
        }

        private void OnCarAttack()
        {
            hintUI.StopArrowBackAndForth(arrow);
            tutCar.OnCarAttack -= OnCarAttack;
            Tween.StopAll(carLight);
            carLight.intensity = _initialLightIntensity;
            _callback?.Invoke();
        }
    }
}
