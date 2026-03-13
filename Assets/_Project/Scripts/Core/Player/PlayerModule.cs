using _Project.Scripts.Core.Modules.Interface;
using Sisus.Init;
using UnityEngine;

namespace _Project.Scripts.Core.Player
{
    public class PlayerModule : MonoBehaviour<PlayerMovementController>, ITimeControllable
    {
        // potentially migrate to global settings?
        [SerializeField] private float fastForwardSpeedMultiplier = 1.5f;
        [SerializeField] private float rewindSpeedMultiplier = 0.5f;
        
        private PlayerMovementController _playerMovementController;
        private Collider _colliderCache;
        
        protected override void Init(PlayerMovementController playerMovementController)
        {
            _playerMovementController = playerMovementController;
        }
        
        public void FastForward()
        {
            _playerMovementController.SpeedMultiplier = fastForwardSpeedMultiplier;
        }
        
        public void Rewind()
        {
            _playerMovementController.SpeedMultiplier = rewindSpeedMultiplier;
        }

        public void CancelRewind()
        {
            _playerMovementController.SpeedMultiplier = 1f;
        }

        public void CancelFastForward()
        {
            _playerMovementController.SpeedMultiplier = 1f;
        }

        public void Anchor(Transform parent)
        {
            transform.SetParent(parent);
        }
    }
}
