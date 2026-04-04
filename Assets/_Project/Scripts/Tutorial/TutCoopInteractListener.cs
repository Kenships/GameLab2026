using System;
using _Project.Scripts.Core.Player;
using UnityEngine;

namespace _Project.Scripts.Tutorial
{
    public class TutCoopInteractListener : MonoBehaviour, ITutorialListener
    {
        [SerializeField] private TutPlayerInteractionController player1;
        [SerializeField] private TutPlayerInteractionController player2;

        private bool _startCheck;
        private Action _callback;
        
        public void Invoke(Action callback)
        {
            _callback = callback;
            _startCheck = true;
            
            player1.OnRewind += TimeControl;
            player2.OnRewind += TimeControl;
            player1.OnFastForward += TimeControl;
            player2.OnFastForward += TimeControl;
        }

        private void TimeControl(PlayerData.PlayerID playerID)
        {
            
        }
    }
}
