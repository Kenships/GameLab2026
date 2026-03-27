using System;
using UnityEngine;

namespace _Project.Scripts.Tutorial
{
    public class TutCarListener : MonoBehaviour, ITutorialListener
    {
        [SerializeField] private TutCar tutCar;

        private Action _callback;
        
        public void Invoke(Action callback)
        {
            _callback = callback;
            
            tutCar.OnCarAttack += OnCarAttack;
        }

        private void OnCarAttack()
        {
            tutCar.OnCarAttack -= OnCarAttack;
            
            _callback?.Invoke();
        }
    }
}
