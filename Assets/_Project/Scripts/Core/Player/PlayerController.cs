using System;
using Sisus.Init;
using UnityEngine;

namespace _Project.Scripts.Core.Player
{
    public class PlayerController : MonoBehaviour<PlayerMovementController, PlayerInteractionController>
    {
        private PlayerMovementController _playerMovementController;
        private PlayerInteractionController _playerInteractionController;
        
        protected override void Init(PlayerMovementController movementController, PlayerInteractionController interactionController)
        {
            _playerMovementController = movementController;
            _playerInteractionController = interactionController;
        }

        private void Update()
        {
            if (_playerInteractionController.IsTimeControlling)
            {
                _playerMovementController.DisableMovement();
            }
            else
            {
                _playerMovementController.EnableMovement();
            }
        }
    }
}
