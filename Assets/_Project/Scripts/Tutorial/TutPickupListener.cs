using System;
using _Project.Scripts.Core.Player;
using UnityEngine;

namespace _Project.Scripts.Tutorial
{
    public class TutPickupListener : MonoBehaviour, ITutorialListener
    {
        [SerializeField] private TutPlayerInteractionController player1;
        [SerializeField] private TutPlayerInteractionController player2;

        private bool _beginCheck;
        
        private Action _callback;
        
        public void Invoke(Action callback)
        {
           player1.OnDrop += OnDrop;
           player2.OnDrop += OnDrop;
           _callback = callback;
        }

        private void OnDrop(PlayerData.PlayerID obj)
        {
            player1.OnDrop -= OnDrop;
            player2.OnDrop -= OnDrop;
            _callback?.Invoke();
        }
    }
}
