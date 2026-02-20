using System.Collections.Generic;
using _Project.Scripts.Core.Grid;
using _Project.Scripts.Core.InputManagement.Interfaces;
using _Project.Scripts.GridObjects.Interface;
using _Project.Scripts.Interaction.Interface;
using Sisus.Init;
using UnityEngine;
using ILogger = _Project.Scripts.Util.Logger.Interface.ILogger;

namespace _Project.Scripts.Core.Player
{
    public class InteractionsWithObject : MonoBehaviour<INESActionReader,IGridService, ILogger>
    {
        [SerializeField] private Transform frontOfPlayer;
        private GameObject _currentIHoldingObject;
        private INESActionReader _inputReader;
        private IGridService _gridService;
        private ILogger _logger;
        
        private readonly List<ITimeControllable> _currentlyTimeControlledObjects = new ();
        
        protected override void Init(INESActionReader nesActionReader, IGridService gridService, ILogger logger)
        {
            _inputReader = nesActionReader;
            _gridService = gridService;
        }
        
        private void OnEnable()
        {
            _inputReader.OnDoubleTapAltInteract += PickUpOrPutDown;
            
            _inputReader.OnHoldInteract += FastForward;
            _inputReader.OnReleaseInteract += CancelFastForward;
            
            _inputReader.OnHoldAltInteract += Rewind;
            _inputReader.OnReleaseAltInteract += CancelRewind;
        }

        //Maybe merge the Cancel interactions into one

        private void CancelFastForward()
        {
            foreach (var timeControllable in _currentlyTimeControlledObjects)
            {
                timeControllable.CancelFastForward();
            }
            
            _currentlyTimeControlledObjects.Clear();
        }
        
        
        private void CancelRewind()
        {
            foreach (var timeControllable in _currentlyTimeControlledObjects)
            {
                timeControllable.CancelRewind();
            }
            _currentlyTimeControlledObjects.Clear();
            
        }

        

        private void OnDisable()
        {
            _inputReader.OnDoubleTapAltInteract -= PickUpOrPutDown;
            
            _inputReader.OnHoldInteract -= FastForward;
            _inputReader.OnReleaseInteract -= CancelFastForward;
            
            _inputReader.OnHoldAltInteract -= Rewind;
            _inputReader.OnReleaseAltInteract -= CancelRewind;
        }
        
        // Double tap A
        private void PickUpOrPutDown()
        {
            // Pick Up
            if (!_currentIHoldingObject)
            {
                GameObject obj = _gridService.GetObjectOnGrid(frontOfPlayer.position);
                if(obj == null) return;
                if (!obj.TryGetComponent(out IHoldable holdable)) return;
                
                holdable.PickUp();
                holdable.Anchor(frontOfPlayer);
                
                _currentIHoldingObject = obj;
            }
            // Put Down
            else
            {
                if (!_currentIHoldingObject.TryGetComponent(out IHoldable holdable))
                {
                    _logger.LogError($"Current Item Held: {_currentIHoldingObject.name} has no IHoldable");
                }
                _gridService.PlaceObjectOnGrid(_currentIHoldingObject, frontOfPlayer.position);
                holdable.Drop();
                
                
                _gridService.PlaceObjectOnGrid(_currentIHoldingObject, frontOfPlayer.position);
                _currentIHoldingObject = null;
            }
        }

        // Hold A
        private void FastForward()
        {
            //Some default logic to determine if Interact is possible right now
            if (!CanInteract())
            {
                return;
            }
            
            GameObject objOnGrid = _gridService.GetObjectOnGrid(frontOfPlayer.position);

            if (objOnGrid && objOnGrid.TryGetComponent(out ITimeControllable timeControllable))
            {
                if (timeControllable.IsWinding)
                {
                    return;
                }

                timeControllable.FastForward();
                _currentlyTimeControlledObjects.Add(timeControllable);
            }
        }

        // Hold B
        private void Rewind()
        {
            //Some default logic to determine if Interact is possible right now
            if (!CanInteract())
            {
                return;
            }
            
            GameObject objOnGrid = _gridService.GetObjectOnGrid(frontOfPlayer.position);

            if (objOnGrid && objOnGrid.TryGetComponent(out ITimeControllable timeControllable))
            {
                if (timeControllable.IsWinding)
                {
                    return;
                }

                timeControllable.Rewind();
                _currentlyTimeControlledObjects.Add(timeControllable);
            }
        }
        
        private bool CanInteract()
        {
            return !_currentIHoldingObject;
        }
    }
}
