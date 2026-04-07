using System;
using _Project.Scripts.Core.Player;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Tutorial
{
    public class TutPickupListener : MonoBehaviour, ITutorialListener
    {
        [SerializeField] private TutPlayerInteractionController player1;
        [SerializeField] private TutPlayerInteractionController player2;
        [SerializeField] private GameObject hintText;
        [SerializeField] private HintUI hintUI;
        [SerializeField] private RawImage arrow;

        private bool requireBothPlayers;

        private Action _callback;

        private bool _player1PickedUp;
        private bool _player2PickedUp;
        
        private void Awake()
        {
            player1.AllowPickUp = false;
            player2.AllowPickUp = false;
        }

        public void SetRequireBothPlayers(bool requireBothPlayers)
        {
            this.requireBothPlayers = requireBothPlayers;
        }

        public void Invoke(Action callback)
        {
            if(requireBothPlayers) hintText.SetActive(true);

            _callback = callback;
            
            player1.AllowPickUp = true;
            player2.AllowPickUp = true;
            
            _player1PickedUp = false;
            _player2PickedUp = false;

            player1.OnPickup += HandlePickup;
            player2.OnPickup += HandlePickup;
        }

        private void OnDestroy()
        {
            if (player1 != null) player1.OnPickup -= HandlePickup;
            if (player2 != null) player2.OnPickup -= HandlePickup;
        }

        private void HandlePickup(PlayerData.PlayerID id)
        {
            hintUI.StopArrowBackAndForth(arrow);
            if (id == PlayerData.PlayerID.Player1) _player1PickedUp = true;
            if (id == PlayerData.PlayerID.Player2) _player2PickedUp = true;

            if (requireBothPlayers)
            {
                if (_player1PickedUp && _player2PickedUp)
                {
                    CompleteTutorialTask();
                }
            }
            else
            {
                CompleteTutorialTask();
            }
        }

        private void CompleteTutorialTask()
        {
            hintText.SetActive(false);

            player1.OnPickup -= HandlePickup;
            player2.OnPickup -= HandlePickup;

            _callback?.Invoke();
        }
    }
}
