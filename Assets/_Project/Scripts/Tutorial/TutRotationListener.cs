using System;
using _Project.Scripts.Core.Player;
using UnityEngine;

namespace _Project.Scripts.Tutorial
{
    public class TutRotationListener : MonoBehaviour, ITutorialListener
    {
        [SerializeField] private TutPlayerInteractionController player1;
        [SerializeField] private TutPlayerInteractionController player2;
        [SerializeField] private GameObject hintText;

        private Action _callback;

        private bool _player1Rotated;
        private bool _player2Rotated;
        
        public void Invoke(Action callback)
        {
            hintText.SetActive(true);
            _callback = callback;
            
            _player1Rotated = false;
            _player2Rotated = false;
            
            player1.OnRotateClockWise += RotateClockWise;
            player2.OnRotateClockWise += RotateClockWise;
            
        }

        private void OnDestroy()
        {
            player1.OnRotateClockWise -= RotateClockWise;
            player2.OnRotateClockWise -= RotateClockWise;
        }

        private void RotateClockWise(PlayerData.PlayerID obj)
        {
            switch (obj)
            {
                case PlayerData.PlayerID.Player1:
                    _player1Rotated = true;
                    break;
                case PlayerData.PlayerID.Player2:
                    _player2Rotated = true;
                    break;
            }

            if (_player1Rotated && _player2Rotated)
            {
                hintText.SetActive(false);
                player1.OnRotateClockWise -= RotateClockWise;
                player2.OnRotateClockWise -= RotateClockWise;
                
                _callback?.Invoke();
            }
        }
    }
}
