using System;
using _Project.Scripts.Core.InputManagement;
using UnityEngine;

namespace _Project.Scripts.Tutorial
{
    public class TutConfirmationListener : MonoBehaviour, ITutorialListener
    {
        [SerializeField] private NESActionReader player1Actions;
        [SerializeField] private NESActionReader player2Actions;

        [SerializeField] private GameObject hintText;
        [SerializeField] private GameObject player1Confirm;
        [SerializeField] private GameObject player2Confirm;

        private bool _player1Confirm;
        private bool _player2Confirm;

        private Action _callback;
        
        public void Invoke(Action callback)
        {
            _callback = callback;
            
            hintText.SetActive(true);
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
            player1Confirm.SetActive(true);

            if (_player2Confirm)
            {
                hintText.SetActive(false);
                player1Confirm.SetActive(false);
                player2Confirm.SetActive(false);
                
                player1Actions.OnTapInteract -= Player1ActionsOnTapInteract;
                player2Actions.OnTapInteract -= Player2ActionsOnTapInteract;
                _callback.Invoke();
            }
        }

        private void Player1ActionsOnTapInteract()
        {
            _player2Confirm = true;
            player2Confirm.SetActive(true);
            
            if (_player1Confirm)
            {
                player1Confirm.SetActive(false);
                player2Confirm.SetActive(false);
                
                player1Actions.OnTapInteract -= Player1ActionsOnTapInteract;
                player2Actions.OnTapInteract -= Player2ActionsOnTapInteract;
                _callback.Invoke();
            }
        }
    }
}
