using System;
using _Project.Scripts.Core.Modules.Interface;
using _Project.Scripts.Core.Player;
using UnityEngine;

namespace _Project.Scripts.Tutorial
{
    public class TutPlayerInteractionController : PlayerInteractionController
    {
        public Action<PlayerData.PlayerID> OnRotateClockWise;
        public Action<PlayerData.PlayerID> OnFastForward;
        public Action<PlayerData.PlayerID> OnRewind;
        public Action<PlayerData.PlayerID> OnPickup;
        public Action<PlayerData.PlayerID> OnDrop;
        
        private PlayerData.PlayerID _playerID;

        protected override void OnAwake()
        {
            base.OnAwake();
            _playerID = GetComponent<PlayerData>().ID;
        }

        protected override void RotateClockWise()
        {
            if (!IsGameTimeFlowing) return;

            if (!_currentIHoldingObject)
            {
                GameObject obj = _gridService.GetObjectOnGrid(frontOfPlayer.position);
                if (!obj)
                {
                    return;
                }
                
                if (!obj.TryGetComponent(out IHoldable holdable))
                {
                    _logger.Log($"Current Item: {obj.name} cannot be rotated");
                    return;
                }
                
                OnRotateClockWise?.Invoke(_playerID);
                holdable.RotateClockWise();
                return;
            }

            OnRotateClockWise?.Invoke(_playerID);
            _currentIHoldingObject.TryGetComponent(out IHoldable currentHoldable);
            currentHoldable.RotateClockWise();
        }

        protected override void PickUpOrPutDown()
        {
            if (!IsGameTimeFlowing) return;

            // Pick Up
            if (!_currentIHoldingObject)
            {
                GameObject obj = _gridService.GetObjectOnGrid(frontOfPlayer.position);
                if(!obj) return;
                if (!obj.TryGetComponent(out IHoldable holdable)) return;
                
                OnPickup?.Invoke(_playerID);
                holdable.PickUp();
                holdable.Anchor(frontOfPlayer);
                
                StartCoroutine(PlayHaptics());

                _currentIHoldingObject = obj;
            }
            // Put Down
            else
            {
                GameObject obj = _gridService.GetObjectOnGrid(frontOfPlayer.position);
                if (obj) return;
                
                if (!_currentIHoldingObject.TryGetComponent(out IHoldable holdable))
                {
                    _logger.LogError($"Current Item Held: {_currentIHoldingObject.name} has no IHoldable");
                }
                
                OnDrop?.Invoke(_playerID);
                _gridService.PlaceObjectOnGrid(_currentIHoldingObject, frontOfPlayer.position);
                holdable.Drop();
                
                StartCoroutine(PlayHaptics());
                
                _currentIHoldingObject = null;
            }

            rebakeNavMesh.Raise();
        }

        protected override void FastForward()
        {
            if (!IsGameTimeFlowing) return;

            //Some default logic to determine if Interact is possible right now
            if (!CanInteract())
            {
                return;
            }
            
            _isFastForwarding = true;
            windVFXController.Show();
            foreach (ITimeControllable controllable in _inRangeTimeControllables)
            {
                OnFastForward?.Invoke(_playerID);
                controllable?.FastForward(PlayerID);
            }
        }

        protected override void Rewind()
        {
            if (!IsGameTimeFlowing) return;

            //Some default logic to determine if Interact is possible right now
            if (!CanInteract())
            {
                return;
            }

            _isRewinding = true;
            windVFXController.Show(WindVFXController.AbilityMode.Rewind);
            

            foreach (ITimeControllable controllable in _inRangeTimeControllables)
            {
                OnRewind?.Invoke(_playerID);
                controllable?.Rewind(PlayerID);
            }
        }
    }
}
