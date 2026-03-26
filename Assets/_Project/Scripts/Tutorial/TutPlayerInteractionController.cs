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
        
        private PlayerData.PlayerID playerID;

        protected override void OnAwake()
        {
            base.OnAwake();
            playerID = GetComponent<PlayerData>().ID;
        }

        protected override void RotateClockWise()
        {
            if (!isTimeFlowing) return;

            if (!_currentIHoldingObject)
            {
                GameObject obj = _gridService.GetObjectOnGrid(frontOfPlayer.position);
                if (!obj.TryGetComponent(out IHoldable holdable))
                {
                    _logger.LogError($"Current Item: {_currentIHoldingObject.name} cannot be rotated");
                    return;
                }
                
                OnRotateClockWise?.Invoke(playerID);
                holdable.RotateClockWise();
                return;
            }

            OnRotateClockWise?.Invoke(playerID);
            _currentIHoldingObject.TryGetComponent(out IHoldable currentHoldable);
            currentHoldable.RotateClockWise();
        }

        protected override void PickUpOrPutDown()
        {
            if (!isTimeFlowing) return;

            // Pick Up
            if (!_currentIHoldingObject)
            {
                GameObject obj = _gridService.GetObjectOnGrid(frontOfPlayer.position);
                if(!obj) return;
                if (!obj.TryGetComponent(out IHoldable holdable)) return;
                
                OnPickup?.Invoke(playerID);
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
                
                OnDrop?.Invoke(playerID);
                _gridService.PlaceObjectOnGrid(_currentIHoldingObject, frontOfPlayer.position);
                holdable.Drop();
                
                StartCoroutine(PlayHaptics());
                
                _currentIHoldingObject = null;
            }

            rebakeNavMesh.Raise();
        }

        protected override void FastForward()
        {
            if (!isTimeFlowing) return;

            //Some default logic to determine if Interact is possible right now
            if (!CanInteract())
            {
                return;
            }
            
            IsTimeControlling = true;
            windVFXController.Show();
            foreach (ITimeControllable controllable in _controllables)
            {
                OnFastForward?.Invoke(playerID);
                controllable?.FastForward();
            }
        }

        protected override void Rewind()
        {
            if (!isTimeFlowing) return;

            //Some default logic to determine if Interact is possible right now
            if (!CanInteract())
            {
                return;
            }

            IsTimeControlling = true;
            windVFXController.Show(WindVFXController.AbilityMode.Rewind);
            

            foreach (ITimeControllable controllable in _controllables)
            {
                OnRewind?.Invoke(playerID);
                controllable?.Rewind();
            }
        }
    }
}
