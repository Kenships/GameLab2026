using _Project.Scripts.Core.Modules.Base_Class;
using _Project.Scripts.Core.Player;
using UnityEngine;

namespace _Project.Scripts.Core.Modules
{
    [RequireComponent(typeof(PlayerMovementController))]
    public class PlayerModule : TimeControllableBase
    {
        // potentially migrate to global settings?
        [SerializeField] private float fastForwardSpeedMultiplier = 1.5f;
        [SerializeField] private float rewindSpeedMultiplier = 0.5f;
        
        private PlayerMovementController _playerMovementController;
        private Collider _colliderCache;

        protected override void OnAwake()
        {
            base.OnAwake();
            _playerMovementController = GetComponent<PlayerMovementController>();

            _fastForwardAction = () => _playerMovementController.SpeedMultiplier = fastForwardSpeedMultiplier;
            _rewindAction = () => _playerMovementController.SpeedMultiplier = rewindSpeedMultiplier;
            _cancelFastForwardAction = () => _playerMovementController.SpeedMultiplier = 1f;
            _cancelRewindAction = () => _playerMovementController.SpeedMultiplier = 1f;
        }
    }
}
