using System;
using _Project.Scripts.Core.InputManagement;
using _Project.Scripts.Multiplayer;
using _Project.Scripts.Util.Timer.Timers;
using UnityEngine;

namespace _Project.Scripts.Tutorial
{
    public class TutMovementListener : MonoBehaviour, ITutorialListener
    {
        private IDevicePairingService _devicePairingService;
        [SerializeField] private float requiredTimeToMove = 2f;
        [SerializeField] private NESActionReader player1Actions;
        [SerializeField] private NESActionReader player2Actions;
        [SerializeField] private GameObject hintText;

        private bool _player1Moved;
        private bool _player2Moved;

        private CountdownTimer _countdownTimer;
        
        public void Invoke(Action callback)
        {
            hintText.SetActive(true);
            player1Actions.OnDPadInput += Player1ActionsOnOnDPadInput;
            player2Actions.OnDPadInput += Player2ActionsOnOnDPadInput;
            
            _countdownTimer = new CountdownTimer(requiredTimeToMove);
            _countdownTimer.OnTimerEnd += callback;
        }

        private void OnDestroy()
        {
            player1Actions.OnDPadInput -= Player1ActionsOnOnDPadInput;
            player2Actions.OnDPadInput -= Player2ActionsOnOnDPadInput;
        }

        private void Player1ActionsOnOnDPadInput(Vector2 arg0)
        {
            _player1Moved = true;

            if (_player2Moved && !_countdownTimer.IsRunning)
            {
                hintText.SetActive(false);
                player1Actions.OnDPadInput -= Player1ActionsOnOnDPadInput;
                player2Actions.OnDPadInput -= Player2ActionsOnOnDPadInput;
                _countdownTimer.Start();
            }
        }

        private void Player2ActionsOnOnDPadInput(Vector2 arg0)
        {
            _player2Moved = true;

            if (_player1Moved && !_countdownTimer.IsRunning)
            {
                hintText.SetActive(false);
                player1Actions.OnDPadInput -= Player1ActionsOnOnDPadInput;
                player2Actions.OnDPadInput -= Player2ActionsOnOnDPadInput;
                _countdownTimer.Start();
            }
        }
    }
}
