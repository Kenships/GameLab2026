using System.Collections.Generic;
using _Project.Scripts.Core.AudioPooling;
using _Project.Scripts.Core.Grid;
using _Project.Scripts.Core.InputManagement.Interfaces;
using _Project.Scripts.Core.Modules.Interface;
using Sisus.Init;
using UnityEngine;
using ILogger = _Project.Scripts.Util.Logger.Interface.ILogger;

namespace _Project.Scripts.Core.Player
{
    [RequireComponent(typeof(RangeDetector))]
    public class PlayerInteractionController : MonoBehaviour<INESActionReader,IGridService, ILogger, AudioPooler>
    {
        [SerializeField] private Transform frontOfPlayer;
        private RangeDetector _rangeDetector;
        private List<ITimeControllable> _controllables = new();
        private GameObject _currentIHoldingObject;
        private INESActionReader _inputReader;
        private IGridService _gridService;
        private ILogger _logger;
        private AudioPooler _audioPooler;

        public bool IsTimeControlling {get; private set;}
        
        public int PlayerID { get; set; }
        
        protected override void Init(INESActionReader nesActionReader, IGridService gridService, ILogger logger, AudioPooler audioPooler)
        {
            _inputReader = nesActionReader;
            _gridService = gridService;
            _rangeDetector = GetComponent<RangeDetector>();
            _audioPooler = audioPooler;
        }
        
        private void OnEnable()
        {
            _inputReader.OnDoubleTapInteract += PickUpOrPutDown;
            
            _inputReader.OnHoldInteract += FastForward;
            _inputReader.OnReleaseInteract += CancelFastForward;
            
            _inputReader.OnDoubleTapAltInteract += RotateClockWise;
            
            _inputReader.OnHoldAltInteract += Rewind;
            _inputReader.OnReleaseAltInteract += CancelRewind;
        }

        private void OnDisable()
        {
            _inputReader.OnDoubleTapInteract -= PickUpOrPutDown;
            
            _inputReader.OnHoldInteract -= FastForward;
            _inputReader.OnReleaseInteract -= CancelFastForward;
            
            _inputReader.OnDoubleTapAltInteract -= RotateClockWise;
            
            _inputReader.OnHoldAltInteract -= Rewind;
            _inputReader.OnReleaseAltInteract -= CancelRewind;
        }

        private void Start()
        {
            _rangeDetector.OnObjectEnter += SelectVisual;
            _rangeDetector.OnObjectExit += DeselectVisual;
        }

        private void RotateClockWise()
        {
            if(!_currentIHoldingObject) return;
            
            _currentIHoldingObject.transform.Rotate(Vector3.up, 90);
        }

        private void FixedUpdate()
        {
            _rangeDetector.GetObjectTypeInRangeNoAlloc(_controllables);
        }

        // Double tap A
        private void PickUpOrPutDown()
        {
            // Pick Up
            if (!_currentIHoldingObject)
            {
                GameObject obj = _gridService.GetObjectOnGrid(frontOfPlayer.position);
                if(!obj) return;
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

            IsTimeControlling = true;

            foreach (ITimeControllable controllable in _controllables)
            {
                controllable?.FastForward();
            }
        }

        private void CancelFastForward()
        {
            foreach (ITimeControllable controllable in _controllables)
            {
                controllable?.CancelFastForward();
            }
            
            IsTimeControlling = false;
        }

        // Hold B
        private void Rewind()
        {
            //Some default logic to determine if Interact is possible right now
            if (!CanInteract())
            {
                return;
            }

            IsTimeControlling = true;

            foreach (ITimeControllable controllable in _controllables)
            {
                controllable?.Rewind();
            }
        }

        private void CancelRewind()
        {
            foreach (ITimeControllable controllable in _controllables)
            {
                controllable?.CancelRewind();
            }
            
            IsTimeControlling = false;
        }
        
        private bool CanInteract()
        {
            return !_currentIHoldingObject && !IsTimeControlling;
        }

        private void SelectVisual(Collider obj)
        {
            if (!obj.TryGetComponent(out IVisualSelectable visualSelectable)) return;
            
            visualSelectable.ShowVisual(PlayerID);
        }
        
        private void DeselectVisual(Collider obj)
        {
            if (!obj.TryGetComponent(out IVisualSelectable visualSelectable)) return;
            visualSelectable.HideVisual(PlayerID);
        }
    }
}
