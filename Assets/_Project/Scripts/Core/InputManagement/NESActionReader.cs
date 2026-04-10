using System;
using _Project.Scripts.Core.InputManagement.Interfaces;
using _Project.Scripts.Core.Player;
using _Project.Scripts.Core.SceneLoading.Interfaces;
using _Project.Scripts.Multiplayer;
using Sisus.Init;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

namespace _Project.Scripts.Core.InputManagement
{
    [RequireComponent(typeof(PlayerData))]
    public class NESActionReader : MonoBehaviour<IDevicePairingService, ISceneFocusRetrieval>, INESActionReader
    {
        public event UnityAction<Vector2> OnNavigate;
        public event UnityAction OnSubmit;
        public event UnityAction OnCancel;
        public event UnityAction OnEscape;
        
        public event UnityAction<Vector2> OnDPadInput;
        public event UnityAction OnHoldInteract;
        public event UnityAction OnReleaseInteract;
        public event UnityAction OnTapInteract;
        public event UnityAction OnHoldAltInteract;
        public event UnityAction OnReleaseAltInteract;
        public event UnityAction OnTapAltInteract;
        
        
        private INESAction _actions;
        private ISceneFocusRetrieval _sceneFocusRetrieval;
        private bool _frameOffset;
        
        protected override void Init(IDevicePairingService devicePairingService, ISceneFocusRetrieval sceneFocusRetrieval)
        {
            if (!devicePairingService.TryGetFor(this, out INESAction action))
            {
                Debug.LogError($"Input Actions Not Found for Player{gameObject.name}, ID: {gameObject.GetComponent<PlayerData>().ID}");
                return;
            }
            _actions = action;
            _sceneFocusRetrieval = sceneFocusRetrieval;
        }

        private void OnEnable()
        {
            _actions.MovePerformed += MoveOnPerformed;
            _actions.MoveCancelled += MoveOnCanceled;
            
            _actions.InteractPerformed += InteractOnPerformed;
            _actions.InteractCancelled += InteractOnCanceled;
            
            _actions.AltInteractPerformed += AltInteractOnPerformed;
            _actions.AltInteractCancelled += AltInteractOnCanceled;
            
            _actions.UINavigatePerformed += NavigateOnPerformed;
            _actions.UISubmitPerformed += SubmitOnPerformed;
            _actions.UICancelPerformed += CancelOnPerformed;
            
            _actions.OverridePerformed += EscapeOnPerformed;
        }

        
        private void OnDisable()
        {
            if (_actions == null) return;
            
            _actions.MovePerformed -= MoveOnPerformed;
            _actions.MoveCancelled -= MoveOnCanceled;
            
            _actions.InteractPerformed -= InteractOnPerformed;
            _actions.InteractCancelled -= InteractOnCanceled;
            
            _actions.AltInteractPerformed -= AltInteractOnPerformed;
            _actions.AltInteractCancelled -= AltInteractOnCanceled;
            
            _actions.UINavigatePerformed -= NavigateOnPerformed;
            _actions.UISubmitPerformed-= SubmitOnPerformed;
            _actions.UICancelPerformed -= CancelOnPerformed;
            
            _actions.OverridePerformed -= EscapeOnPerformed;
        }

        private void EscapeOnPerformed(InputAction.CallbackContext obj)
        {
            if (!IsFocused()) return;
            if (_frameOffset)
            {
                _frameOffset = false;
                return;
            }
            TurnItOnAndOffAgainLol();
            OnEscape?.Invoke();
        }

        
        private void CancelOnPerformed(InputAction.CallbackContext obj)
        {
            if (!IsFocused()) return;
            if (_frameOffset)
            {
                _frameOffset = false;
                return;
            }
            OnCancel?.Invoke();
        }

        private void SubmitOnPerformed(InputAction.CallbackContext ctx)
        {
            if (!IsFocused()) return;
            if (_frameOffset)
            {
                _frameOffset = false;
                return;
            }
            OnSubmit?.Invoke();
        }

        private void NavigateOnPerformed(InputAction.CallbackContext ctx)
        {
            if (!IsFocused()) return;
            if (_frameOffset)
            {
                _frameOffset = false;
                return;
            }
            OnNavigate?.Invoke(ctx.ReadValue<Vector2>());
        }

        private void AltInteractOnCanceled(InputAction.CallbackContext ctx)
        {
            if (ctx.interaction is HoldInteraction)
            {
                OnReleaseAltInteract?.Invoke();
            }
        }

        private void AltInteractOnPerformed(InputAction.CallbackContext ctx)
        {
            if (!IsFocused()) return;
            if (_frameOffset)
            {
                _frameOffset = false;
                return;
            }
            
            if (ctx.interaction is TapInteraction)
            {
                OnTapAltInteract?.Invoke();
            }
            else if (ctx.interaction is HoldInteraction)
            {
                OnHoldAltInteract?.Invoke();
            }
        }

        private void InteractOnCanceled(InputAction.CallbackContext ctx)
        {
            if (ctx.interaction is HoldInteraction)
            {
                OnReleaseInteract?.Invoke();
            }
        }

        private void InteractOnPerformed(InputAction.CallbackContext ctx)
        {
            if (!IsFocused()) return;
            if (_frameOffset)
            {
                _frameOffset = false;
                return;
            }
            
            if (ctx.interaction is TapInteraction)
            {
                OnTapInteract?.Invoke();
            }
            else if (ctx.interaction is HoldInteraction)
            {
                OnHoldInteract?.Invoke();
            }
        }

        private void MoveOnCanceled(InputAction.CallbackContext ctx)
        {
            OnDPadInput?.Invoke(ctx.ReadValue<Vector2>());
        }

        private void MoveOnPerformed(InputAction.CallbackContext ctx)
        {
            if (!IsFocused()) return;
            if (_frameOffset)
            {
                _frameOffset = false;
                return;
            }
            
            OnDPadInput?.Invoke(ctx.ReadValue<Vector2>());
        }

        private void TurnItOnAndOffAgainLol()
        {
            _actions.Disable();
            _actions.Enable();
            _frameOffset = true;
        }
        
        public bool TryGetGamePad(out Gamepad gamePad)
        {
            gamePad = null;

            if (_actions.devices == null)
            {
                return false;
            }
            
            foreach (InputDevice device in _actions.devices)
                if (device is Gamepad gp)
                    gamePad = gp;

            return gamePad != null;
        }

        private bool IsFocused()
        {
            return _sceneFocusRetrieval.IsFocused(gameObject.scene.buildIndex);
        }
    }
}
