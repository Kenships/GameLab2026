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
    public class NESActionReader : MonoBehaviour<IDevicePairingService, ISceneFocusRetrieval>, INESActionReader, INESUIReader
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
        
        
        private NESActions _actions;
        private ISceneFocusRetrieval _sceneFocusRetrieval;
        private bool _frameOffset;
        
        protected override void Init(IDevicePairingService devicePairingService, ISceneFocusRetrieval sceneFocusRetrieval)
        {
            if (!devicePairingService.TryGetFor(this, out NESActions action))
            {
                Debug.LogError($"Input Actions Not Found for Player{gameObject.name}, ID: {gameObject.GetComponent<PlayerData>().ID}");
                return;
            }
            _actions = action;
            _sceneFocusRetrieval = sceneFocusRetrieval;
        }
        
        
        private void OnEnable()
        {
            _actions.Enable();
            _actions.Player.Move.performed += MoveOnPerformed;
            _actions.Player.Move.canceled += MoveOnCanceled;
            
            _actions.Player.Interact.performed += InteractOnPerformed;
            _actions.Player.Interact.canceled += InteractOnCanceled;
            
            _actions.Player.AltInteract.performed += AltInteractOnPerformed;
            _actions.Player.AltInteract.canceled += AltInteractOnCanceled;
            
            _actions.UI.Navigate.performed += NavigateOnPerformed;
            _actions.UI.Submit.performed += SubmitOnPerformed;
            _actions.UI.Cancel.performed += CancelOnPerformed;
            
            _actions.Override.Escape.performed += EscapeOnPerformed;
        }

        
        private void OnDisable()
        {
            if (_actions == null) return;
            
            _actions.Player.Move.performed -= MoveOnPerformed;
            _actions.Player.Move.canceled -= MoveOnCanceled;
            
            _actions.Player.Interact.performed -= InteractOnPerformed;
            _actions.Player.Interact.canceled -= InteractOnCanceled;
            
            _actions.Player.AltInteract.performed -= AltInteractOnPerformed;
            _actions.Player.AltInteract.canceled -= AltInteractOnCanceled;
            
            _actions.UI.Navigate.performed -= NavigateOnPerformed;
            _actions.UI.Submit.performed -= SubmitOnPerformed;
            _actions.UI.Cancel.performed -= CancelOnPerformed;
            
            _actions.Override.Escape.performed -= EscapeOnPerformed;
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
