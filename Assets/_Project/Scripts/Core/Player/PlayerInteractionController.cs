using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Core.AudioPooling;
using _Project.Scripts.Core.Grid;
using _Project.Scripts.Core.InputManagement.Interfaces;
using _Project.Scripts.Core.Modules;
using _Project.Scripts.Core.Modules.Interface;
using Obvious.Soap;
using Sisus.Init;
using UnityEngine;
using UnityEngine.InputSystem;
using ILogger = _Project.Scripts.Util.Logger.Interface.ILogger;

namespace _Project.Scripts.Core.Player
{
    [RequireComponent(typeof(RangeDetector))]
    public class PlayerInteractionController : MonoBehaviour<INESActionReader, IGridService, ILogger, AudioPooler>
    {
        public static bool IsGameTimeFlowing = true;

        [Header("Haptics Settings")] [SerializeField]
        private FloatVariable hapticsIntensity;
        [SerializeField] private float lowFrequencyHapticIntensity = 0.6f;
        [SerializeField] private float highFrequencyHapticIntensity = .2f;
        [SerializeField] private float hapticsDuration = 0.12f;
        
        [Header("Holding Settings")]
        [SerializeField] protected bool allowRotationWhenHolding = false;

        [Header("References")]
        [SerializeField] protected Transform frontOfPlayer;
        [SerializeField] protected Transform pickupAnchor;

        [SerializeField] protected InteractionVFXController windVFXController;

        [Header("EventObjects")]
        [SerializeField]
        protected ScriptableEventNoParam rebakeNavMesh;

        protected RangeDetector _rangeDetector;
        protected List<ITimeControllable> _inRangeTimeControllables = new();
        protected bool _isFastForwarding;
        protected bool _isRewinding;
        
        protected GameObject _currentIHoldingObject;
        protected INESActionReader _inputReader;
        protected IGridService _gridService;
        protected ILogger _logger;
        protected AudioPooler _audioPooler;
        protected Gamepad _gamePad;
        

        public bool IsTimeControlling => _isFastForwarding || _isRewinding;
        public bool IsHoldingObject => _currentIHoldingObject != null;

        public PlayerData.PlayerID PlayerID { get; set; }

        protected override void Init(INESActionReader nesActionReader, IGridService gridService, ILogger logger,
            AudioPooler audioPooler)
        {
            _logger = logger;
            _inputReader = nesActionReader;
            _gridService = gridService;
            _rangeDetector = GetComponent<RangeDetector>();
            _audioPooler = audioPooler;
            PlayerID = GetComponent<PlayerData>().ID;
        }

        private void OnEnable()
        {
            _inputReader.OnTapInteract += RotateClockWise;

            _inputReader.OnHoldInteract += FastForward;
            _inputReader.OnReleaseInteract += CancelFastForward;

            _inputReader.OnTapAltInteract += PickUpOrPutDown;

            _inputReader.OnHoldAltInteract += Rewind;
            _inputReader.OnReleaseAltInteract += CancelRewind;

            _rangeDetector.OnObjectEnter += RangeDetectorOnObjectEnter;
            _rangeDetector.OnObjectExit += RangeDetectorOnObjectExit;
        }

        private void OnDisable()
        {
            if (_inputReader == null)
                return;

            _gamePad?.SetMotorSpeeds(0,0);
            
            _inputReader.OnTapInteract -= RotateClockWise;

            _inputReader.OnHoldInteract -= FastForward;
            _inputReader.OnReleaseInteract -= CancelFastForward;

            _inputReader.OnTapAltInteract -= PickUpOrPutDown;

            _inputReader.OnHoldAltInteract -= Rewind;
            _inputReader.OnReleaseAltInteract -= CancelRewind;

            _rangeDetector.OnObjectEnter -= RangeDetectorOnObjectEnter;
            _rangeDetector.OnObjectExit -= RangeDetectorOnObjectExit;
        }
        
        private void RangeDetectorOnObjectEnter(Collider obj)
        {
            SelectVisual(obj);
            
            if (!obj.TryGetComponent(out ITimeControllable timeControllable))
            {
                return;
            }

            if (_isFastForwarding)
            {
                timeControllable.FastForward(PlayerID);
            }

            if (_isRewinding)
            {
                timeControllable.Rewind(PlayerID);
            }
        }
        
        private void RangeDetectorOnObjectExit(Collider obj)
        {
            DeselectVisual(obj);

            if (!obj.TryGetComponent(out ITimeControllable timeControllable))
            {
                return;
            }

            if (_isFastForwarding)
            {
                timeControllable.CancelFastForward(PlayerID);
            }

            if (_isRewinding)
            {
                timeControllable.CancelRewind(PlayerID);
            }
        }

        protected virtual void RotateClockWise()
        {
            if (!IsGameTimeFlowing)
                return;

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

                StartCoroutine(RotateHaptics());
                holdable.RotateClockWise();
                return;
            }

            if (allowRotationWhenHolding)
            {
                _currentIHoldingObject.TryGetComponent(out IHoldable currentHoldable);
                StartCoroutine(RotateHaptics());
                currentHoldable.RotateClockWise();
            }
        }

        private void FixedUpdate()
        {
            _rangeDetector.GetObjectTypeInRangeNoAlloc(_inRangeTimeControllables);
        }

        // Double tap A
        protected virtual void PickUpOrPutDown()
        {
            if (!IsGameTimeFlowing)
                return;

            // Pick Up
            if (!_currentIHoldingObject)
            {
                GameObject obj = _gridService.GetObjectOnGrid(frontOfPlayer.position);
                if (!obj)
                    return;
                if (!obj.TryGetComponent(out IHoldable holdable))
                    return;
                
                holdable.PickUp();
                holdable.Anchor(pickupAnchor);

                StartCoroutine(PickupHaptics());

                windVFXController.ShowHeldObject(obj);
                _currentIHoldingObject = obj;
                if (obj.TryGetComponent(out LazerCannon cannon))
                {
                    StartCoroutine(LazerCannonRumble(cannon));
                }
                
            }
            // Put Down
            else
            {
                GameObject obj = _gridService.GetObjectOnGrid(frontOfPlayer.position);
                if (obj)
                    return;

                if (!_currentIHoldingObject.TryGetComponent(out IHoldable holdable))
                {
                    _logger.LogError($"Current Item Held: {_currentIHoldingObject.name} has no IHoldable");
                }

                _gridService.PlaceObjectOnGrid(_currentIHoldingObject, frontOfPlayer.position);
                holdable.Drop();

                StartCoroutine(PickupHaptics());

                windVFXController.HideHeldObject();
                _currentIHoldingObject = null;
            }
        }

        // Hold A
        protected virtual void FastForward()
        {
            if (!IsGameTimeFlowing)
                return;

            //Some default logic to determine if Interact is possible right now
            if (!CanInteract())
            {
                return;
            }

            StartCoroutine(FastForwardHaptics());
            
            _isFastForwarding = true;
            windVFXController.ShowWind();
            foreach (ITimeControllable controllable in _inRangeTimeControllables)
            {
                controllable?.FastForward(PlayerID);
            }
        }

        private void CancelFastForward()
        {
            foreach (ITimeControllable controllable in _inRangeTimeControllables)
            {
                controllable?.CancelFastForward(PlayerID);
            }

            windVFXController.HideWind();
            _isFastForwarding = false;
        }

        // Hold B
        protected virtual void Rewind()
        {
            if (!IsGameTimeFlowing)
                return;

            //Some default logic to determine if Interact is possible right now
            if (!CanInteract())
            {
                return;
            }
            
            StartCoroutine(RewindHaptics());

            _isRewinding = true;
            windVFXController.ShowWind(InteractionVFXController.AbilityMode.Rewind);

            foreach (ITimeControllable controllable in _inRangeTimeControllables)
            {
                controllable?.Rewind(PlayerID);
            }
        }

        private void CancelRewind()
        {
            foreach (ITimeControllable controllable in _inRangeTimeControllables)
            {
                controllable?.CancelRewind(PlayerID);
            }

            windVFXController.HideWind();
            _isRewinding = false;
        }

        protected bool CanInteract()
        {
            return !_currentIHoldingObject && !IsTimeControlling;
        }

        private void SelectVisual(Collider obj)
        {
            if (!obj)
                return;

            if (!obj.TryGetComponent(out IVisualSelectable visualSelectable))
                return;

            visualSelectable.ShowVisual(PlayerID);
        }

        private void DeselectVisual(Collider obj)
        {
            if (!obj)
                return;

            if (!obj.TryGetComponent(out IVisualSelectable visualSelectable))
                return;
            visualSelectable.HideVisual(PlayerID);
        }

        protected IEnumerator FastForwardHaptics()
        {
            yield return PlayHaptics(0, 1f);
        }

        protected IEnumerator RewindHaptics()
        {
            yield return PlayHaptics(1f, 0);
        }

        protected IEnumerator RotateHaptics()
        {
            yield return PlayHaptics(highFrequencyHapticIntensity, lowFrequencyHapticIntensity);
        }
        
        protected IEnumerator PickupHaptics()
        {
            yield return PlayHaptics(lowFrequencyHapticIntensity, highFrequencyHapticIntensity);
        }

        private IEnumerator PlayHaptics(float lowFrequency, float highFrequency)
        {
            if (_gamePad == null && !_inputReader.TryGetGamePad(out _gamePad))
                yield break;

            _gamePad.SetMotorSpeeds(lowFrequency * hapticsIntensity.Value, highFrequency * hapticsIntensity.Value);

            float timer = hapticsDuration;

            while (timer >= 0)
            {
                timer -= Time.unscaledDeltaTime;
                yield return null;
            }

            _gamePad.SetMotorSpeeds(0, 0);
        }

        private IEnumerator LazerCannonRumble(LazerCannon cannon)
        {
            if (_gamePad == null && !_inputReader.TryGetGamePad(out _gamePad))
                yield break;

            while (_currentIHoldingObject == cannon.gameObject)
            {
                if(cannon.IsAttacking)
                {
                    _gamePad.SetMotorSpeeds(.2f * hapticsIntensity.Value, 1f * hapticsIntensity.Value);
                }
                else
                {
                    _gamePad.SetMotorSpeeds(0, 0);
                }
                yield return null;
            }

            _gamePad.SetMotorSpeeds(0, 0);
        }
    }
}
