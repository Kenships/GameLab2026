using System;
using _Project.Scripts.Core.Player;
using UnityEngine;

namespace _Project.Scripts.Tutorial
{
    public class TutTimeControlListener : MonoBehaviour, ITutorialListener
    {
        private enum ListeningType
        {
            FastForward,
            Rewind,
            Both
        }
        [SerializeField] private ListeningType listeningType;
        [SerializeField] private TutPlayerInteractionController player1;
        [SerializeField] private TutPlayerInteractionController player2;
        [SerializeField] private GameObject hintText;

        private Action _callback;
        
        private bool _player1TimeControlled;
        private bool _player2TimeControlled;
        
        private void Awake()
        {
            player1.AllowTimeControl = false;
            player2.AllowTimeControl = false;
        }
        
        public void Invoke(Action callback)
        {
            hintText.SetActive(true);
            _callback = callback;
            
            player1.AllowTimeControl = true;
            player2.AllowTimeControl = true;
            
            _player1TimeControlled = false;
            _player2TimeControlled = false;

            switch (listeningType)
            {
                case ListeningType.FastForward:
                    player1.OnFastForward += OnTimeControl;
                    player2.OnFastForward += OnTimeControl;
                    break;
                case ListeningType.Rewind:
                    player1.OnRewind += OnTimeControl;
                    player2.OnRewind += OnTimeControl;
                    break;
                case ListeningType.Both:
                    player1.OnFastForward += OnTimeControl;
                    player2.OnFastForward += OnTimeControl;
                    player1.OnRewind += OnTimeControl;
                    player2.OnRewind += OnTimeControl;
                    break;
            }
        }

        private void OnDestroy()
        {
            player1.OnFastForward -= OnTimeControl;
            player2.OnFastForward -= OnTimeControl;
            player1.OnRewind -= OnTimeControl;
            player2.OnRewind -= OnTimeControl;
        }

        private void OnTimeControl(PlayerData.PlayerID playerID)
        {
            switch (playerID)
            {
                case PlayerData.PlayerID.Player1:
                    _player1TimeControlled = true;
                    break;
                case PlayerData.PlayerID.Player2:
                    _player2TimeControlled = true;
                    break;
            }

            if (_player1TimeControlled && _player2TimeControlled)
            {
                hintText.SetActive(false);
                player1.OnFastForward -= OnTimeControl;
                player2.OnFastForward -= OnTimeControl;
                player1.OnRewind -= OnTimeControl;
                player2.OnRewind -= OnTimeControl;
                _callback?.Invoke();
            }
        }
    }
}
