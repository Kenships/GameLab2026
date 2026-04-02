using System;
using Obvious.Soap;
using UnityEngine;

namespace _Project.Scripts.Tutorial
{
    public class TutRoundEndListener : MonoBehaviour, ITutorialListener
    {
        [SerializeField] private ScriptableEventNoParam victoryEvent;

        private Action _callback;
        public void Invoke(Action callback)
        {
            _callback = callback;
            victoryEvent.OnRaised += VictoryEventOnOnRaised;
        }

        private void OnDestroy()
        {
            victoryEvent.OnRaised -= VictoryEventOnOnRaised;
        }

        private void VictoryEventOnOnRaised()
        {
            victoryEvent.OnRaised -= VictoryEventOnOnRaised;
            _callback?.Invoke();
        }
    }
}
