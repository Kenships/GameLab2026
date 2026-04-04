using System;
using _Project.Scripts.Core.Modules.Base_Class;
using _Project.Scripts.Core.Player;
using UnityEngine;

namespace _Project.Scripts.Tutorial
{
    public class TutCoopInteractListener : MonoBehaviour, ITutorialListener
    {
        [SerializeField] private TutPlayerInteractionController player1;
        [SerializeField] private TutPlayerInteractionController player2;
        
        private Action _callback;
        
        public void Invoke(Action callback)
        {
            _callback = callback;
            
            player1.OnRewind += TimeControl;
            player2.OnRewind += TimeControl;
            player1.OnFastForward += TimeControl;
            player2.OnFastForward += TimeControl;
        }

        private void TimeControl(PlayerData.PlayerID playerID)
        {
            if (player1.HeldModule && player1.HeldModule.GetComponent<Module>().IsTimeControlling
                || (player2.HeldModule && player2.HeldModule.GetComponent<Module>().IsTimeControlling))
            {
                player1.OnRewind -= TimeControl;
                player2.OnRewind -= TimeControl;
                player1.OnFastForward -= TimeControl;
                player2.OnFastForward -= TimeControl;
                
                _callback?.Invoke();
            }
        }
    }
}
