using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Core.AudioPooling;
using _Project.Scripts.Core.Grid;
using _Project.Scripts.Core.InputManagement.Interfaces;
using _Project.Scripts.Core.Modules.Interface;
using Obvious.Soap;
using Sisus.Init;
using UnityEngine;
using UnityEngine.InputSystem;
using ILogger = _Project.Scripts.Util.Logger.Interface.ILogger;

namespace _Project.Scripts.Core.Player
{
    [RequireComponent(typeof(RangeDetector))]
    public class PlayerInteractionController : MonoBehaviour<INESActionReader,IGridService, ILogger, AudioPooler>
    {
        [Header("Haptics Settings")]
        [SerializeField] private float lowFrequencyHapticIntensity = 0.6f;
        [SerializeField] private float highFrequencyHapticIntensity = .2f;
        [SerializeField] private float hapticsDuration = 0.12f;
        
        [Header("References")]
        [SerializeField] private Transform frontOfPlayer;
        [SerializeField] private WindVFXController windVFXController;
        [Header("EventObjects")]
        [SerializeField] private ScriptableEventNoParam rebakeNavMesh;
        
        private RangeDetector _rangeDetector;
        private List<ITimeControllable> _controllables = new();
        private GameObject _currentIHoldingObject;
        private INESActionReader _inputReader;
        private IGridService _gridService;
        private ILogger _logger;
        private AudioPooler _audioPooler;
        private Gamepad _gamePad;
        public static bool isTimeFlowing = true;

        public bool IsTimeControlling {get; private set;}
        
        public PlayerData.PlayerID PlayerID { get; set; }
        
        protected override void Init(INESActionReader nesActionReader, IGridService gridService, ILogger logger, AudioPooler audioPooler)
        {
            _inputReader = nesActionReader;
            _gridService = gridService;
            _rangeDetector = GetComponent<RangeDetector>();
            _audioPooler = audioPooler;
            PlayerID = GetComponent<PlayerData>().ID;
            
        }
        
        private void OnEnable()
        {
            _inputReader.OnTapInteract += PickUpOrPutDown;
            
            _inputReader.OnHoldInteract += FastForward;
            _inputReader.OnReleaseInteract += CancelFastForward;
            
            _inputReader.OnTapAltInteract += RotateClockWise;
            
            _inputReader.OnHoldAltInteract += Rewind;
            _inputReader.OnReleaseAltInteract += CancelRewind;
            
            _rangeDetector.OnObjectEnter += SelectVisual;
            _rangeDetector.OnObjectExit += DeselectVisual;
            _rangeDetector.OnObjectExit += CancelFastForward;
            _rangeDetector.OnObjectExit += CancelRewind;
        }

        private void OnDisable()
        {
            if (_inputReader == null) return;
            
            _inputReader.OnTapInteract -= PickUpOrPutDown;
            
            _inputReader.OnHoldInteract -= FastForward;
            _inputReader.OnReleaseInteract -= CancelFastForward;
            
            _inputReader.OnTapAltInteract -= RotateClockWise;
            
            _inputReader.OnHoldAltInteract -= Rewind;
            _inputReader.OnReleaseAltInteract -= CancelRewind;
            
            _rangeDetector.OnObjectEnter -= SelectVisual;
            _rangeDetector.OnObjectExit -= DeselectVisual;
            _rangeDetector.OnObjectExit -= CancelFastForward;
            _rangeDetector.OnObjectExit -= CancelRewind;
        }
        

        private void RotateClockWise()
        {
            if (!isTimeFlowing) return;

            if (!_currentIHoldingObject)
            {
                GameObject obj = _gridService.GetObjectOnGrid(frontOfPlayer.position);
                if(!obj || obj.tag.Equals("NonRotatable")) return;
                
                obj.transform.Rotate(Vector3.up, 90);
                return;
            }
            
            _currentIHoldingObject.transform.Rotate(Vector3.up, 90);
        }

        private void FixedUpdate()
        {
            _rangeDetector.GetObjectTypeInRangeNoAlloc(_controllables);
        }

        // Double tap A
        private void PickUpOrPutDown()
        {
            if (!isTimeFlowing) return;

            // Pick Up
            if (!_currentIHoldingObject)
            {
                GameObject obj = _gridService.GetObjectOnGrid(frontOfPlayer.position);
                if(!obj) return;
                if (!obj.TryGetComponent(out IHoldable holdable)) return;
                
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
                
                _gridService.PlaceObjectOnGrid(_currentIHoldingObject, frontOfPlayer.position);
                holdable.Drop();
                
                StartCoroutine(PlayHaptics());
                
                _currentIHoldingObject = null;
            }

            rebakeNavMesh.Raise();
        }

        // Hold A
        private void FastForward()
        {
            if (!isTimeFlowing) return;

            //Some default logic to determine if Interact is possible right now
            if (!CanInteract())
            {
                return;
            }
            
            IsTimeControlling = true;
            windVFXController.Show(WindVFXController.AbilityMode.FastForward);
            foreach (ITimeControllable controllable in _controllables)
            {
                controllable?.FastForward();
            }
        }

        private void CancelFastForward(Collider obj)
        {
            
            CancelFastForward();
        }

        private void CancelFastForward()
        {
            foreach (ITimeControllable controllable in _controllables)
            {
                controllable?.CancelFastForward();
            }

            windVFXController.Hide();
            IsTimeControlling = false;
        }

        // Hold B
        private void Rewind()
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
                controllable?.Rewind();
            }
        }
        
        private void CancelRewind(Collider obj)
        {
            CancelRewind();
        }

        private void CancelRewind()
        {
            foreach (ITimeControllable controllable in _controllables)
            {
                controllable?.CancelRewind();
            }

            windVFXController.Hide();
            IsTimeControlling = false;
        }
        
        private bool CanInteract()
        {
            return !_currentIHoldingObject && !IsTimeControlling;
        }

        private void SelectVisual(Collider obj)
        {
            if(!obj) return;
            
            if (!obj.TryGetComponent(out IVisualSelectable visualSelectable)) return;
            
            visualSelectable.ShowVisual(PlayerID);
        }
        
        private void DeselectVisual(Collider obj)
        {
            if(!obj) return;
            
            if (!obj.TryGetComponent(out IVisualSelectable visualSelectable)) return;
            visualSelectable.HideVisual(PlayerID);
        }

        private IEnumerator PlayHaptics()
        {
            if (!_inputReader.TryGetGamePad(out _gamePad))
                yield break;
            
            _gamePad.SetMotorSpeeds(lowFrequencyHapticIntensity, highFrequencyHapticIntensity);

            float timer = hapticsDuration;

            while (timer >= 0)
            {
                timer -= Time.unscaledDeltaTime;
                yield return null;
            }
            
            _gamePad.SetMotorSpeeds(0, 0);
        }
    }
}
