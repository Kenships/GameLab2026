using System;
using _Project.Scripts.Core.InputManagement;
using UnityEngine;

namespace _Project.Scripts.Tutorial
{
    public class TutConfirmationListener : MonoBehaviour, ITutorialListener
    {
        [SerializeField] private NESActionReader player1Actions;
        [SerializeField] private NESActionReader player2Actions;

        private bool _player1Confirm;
        private bool _player2Confirm;

        private Action _callback;
        
        public void Invoke(Action callback)
        {
            _callback = callback;
            
            _player1Confirm = false;
            _player2Confirm = false;
            
            player1Actions.OnTapInteract += Player1ActionsOnTapInteract;
            player2Actions.OnTapInteract += Player2ActionsOnTapInteract;
        }

        private void OnDestroy()
        {
            player1Actions.OnTapInteract -= Player1ActionsOnTapInteract;
            player2Actions.OnTapInteract -= Player2ActionsOnTapInteract;
        }

        private void Player2ActionsOnTapInteract()
        {
            _player1Confirm = true;

            if (_player2Confirm)
            {
                player1Actions.OnTapInteract -= Player1ActionsOnTapInteract;
                player2Actions.OnTapInteract -= Player2ActionsOnTapInteract;
                _callback.Invoke();
            }
        }

        private void Player1ActionsOnTapInteract()
        {
            _player2Confirm = true;

            if (_player1Confirm)
            {
                player1Actions.OnTapInteract -= Player1ActionsOnTapInteract;
                player2Actions.OnTapInteract -= Player2ActionsOnTapInteract;
                _callback.Invoke();
            }
        }
    }
}
