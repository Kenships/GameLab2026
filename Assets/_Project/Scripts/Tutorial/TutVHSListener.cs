using System;
using _Project.Scripts.Core;
using _Project.Scripts.Core.HealthManagement;
using _Project.Scripts.Core.Modules;
using Obvious.Soap;
using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Tutorial
{
    public class TutVHSListener : MonoBehaviour, ITutorialListener
    {
        [SerializeField] private VHSModule vhsModule;
        //[SerializeField] private Light vhsLight;
        [SerializeField] private HintUI hintUI;
        [SerializeField] private RawImage arrow;

        private Action _callback;
        
        private Health _health;
        
        //private float _initialLightIntensity;
        
        private void Awake()
        {
            _health = vhsModule.GetComponent<Health>();
        }

        private void OnDestroy()
        {
            _health.OnFullHp -= VhsHPFullOnRaised;
            //Tween.StopAll(vhsLight);
        }

        public void Invoke(Action callback)
        {
            _callback = callback;
            //_initialLightIntensity = vhsLight.intensity;
            arrow.enabled = true;
            hintUI.PlayArrowBackAndForth(arrow);

            //PulseLight();

            _health.OnFullHp += VhsHPFullOnRaised;
        }

        //private void PulseLight()
        //{
            //Tween.LightIntensity(
                //target: vhsLight,
                //startValue: _initialLightIntensity,
                //endValue: 15f,
                //duration: 1f,
                //ease: Ease.InOutQuad,
                //cycleMode: CycleMode.Yoyo)
                //.SetRemainingCycles(-1);
        //}

        private void VhsHPFullOnRaised()
        {
            //Tween.StopAll(vhsLight);
            //vhsLight.intensity = _initialLightIntensity;
            arrow.enabled = false;
            hintUI.StopArrowBackAndForth(arrow);
            _health.OnFullHp -= VhsHPFullOnRaised;
            _callback?.Invoke();
        }
    }
}
