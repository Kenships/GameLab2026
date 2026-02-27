using _Project.Scripts.Core.Grid;
using _Project.Scripts.Core.InputManagement.Interfaces;
using Sisus.Init;
using UnityEngine;
using ILogger = _Project.Scripts.Util.Logger.Interface.ILogger;
using _Project.Scripts.Core.AudioPooling;
using AudioType = _Project.Scripts.Core.AudioPooling.Interface.AudioType;

namespace _Project.Scripts.Core.Player
{
    public class InteractionsWithObject : MonoBehaviour<INESActionReader,IGridService, ILogger, AudioPooler>
    {
        [SerializeField] private Transform frontOfPlayer;
        private GameObject _currentIHoldingObject;
        private INESActionReader _inputReader;
        private IGridService _gridService;
        private ILogger _logger;
        private AudioPooler _audioPooler;

        [Header("Audio")]
        [SerializeField] private AudioClip pickUp, place, fastForward, rewind;

        protected override void Init(INESActionReader nesActionReader, IGridService gridService, ILogger logger, AudioPooler audioPooler)
        {
            _inputReader = nesActionReader;
            _gridService = gridService;
            _audioPooler = audioPooler;
        }
        
        private void OnEnable()
        {
            _inputReader.OnDoubleTapAltInteract += PickUpOrPutDown;
            
            _inputReader.OnHoldInteract += FastForward;
            
            _inputReader.OnHoldAltInteract += Rewind;
        }

        private void OnDisable()
        {
            _inputReader.OnDoubleTapAltInteract -= PickUpOrPutDown;
            
            _inputReader.OnHoldInteract -= FastForward;
            
            _inputReader.OnHoldAltInteract -= Rewind;
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

                _audioPooler.New2DAudio(pickUp).OnChannel(AudioType.Sfx).Play();
                
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

                _audioPooler.New2DAudio(place).OnChannel(AudioType.Sfx).Play();

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

            if (objOnGrid && objOnGrid.TryGetComponent(out Module module))
            {
                _audioPooler.New2DAudio(fastForward).OnChannel(AudioType.Sfx).Play();
                module.FastForward();
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

            if (objOnGrid && objOnGrid.TryGetComponent(out Module module))
            {
                _audioPooler.New2DAudio(rewind).OnChannel(AudioType.Sfx).Play();
                module.Rewind();
            }
        }
        
        private bool CanInteract()
        {
            return !_currentIHoldingObject;
        }
    }
}
