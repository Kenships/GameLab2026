using System;
using _Project.Scripts.Core.InputManagement.Interfaces;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

namespace _Project.Scripts.Core.InputManagement
{
    [RequireComponent(typeof(PlayerInput))]
    public class NESActionReader : MonoBehaviour, INESActionReader
    {
        public event UnityAction<Vector2> OnDPadInput;
        public event UnityAction OnHoldInteract;
        public event UnityAction OnReleaseInteract;
        public event UnityAction OnDoubleTapInteract;
        public event UnityAction OnHoldAltInteract;
        public event UnityAction OnReleaseAltInteract;
        public event UnityAction OnDoubleTapAltInteract;

        private PlayerInput _playerInput;
        private NESActions _actions;

        private InputAction _dpad;
        private InputAction _interact;
        private InputAction _altInteract;

        private void Awake()
        {
            if (!_playerInput)
                _playerInput = GetComponent<PlayerInput>();
            _actions = new NESActions();

            _playerInput.actions = _actions.asset;

            _actions.Enable();
        }

        private void Start()
        {
            _actions.Player.Move.performed += MoveOnPerformed;
            _actions.Player.Move.canceled += MoveOnCanceled;
            
            _actions.Player.Interact.performed += InteractOnPerformed;
            _actions.Player.Interact.canceled += InteractOnCanceled;
            
            _actions.Player.AltInteract.performed += AltInteractOnPerformed;
            _actions.Player.AltInteract.canceled += AltInteractOnCanceled;
        }

        private void AltInteractOnCanceled(InputAction.CallbackContext ctx)
        {
            if (ctx.interaction is HoldInteraction)
            {
                Debug.Log("AltInteract Canceled");
                OnReleaseAltInteract?.Invoke();
            }
        }

        private void AltInteractOnPerformed(InputAction.CallbackContext ctx)
        {
            if (ctx.interaction is HoldInteraction)
            {
                Debug.Log("Holding alt interaction");
                OnHoldAltInteract?.Invoke();
            }
            else if (ctx.interaction is MultiTapInteraction)
            {
                Debug.Log("Multi-taping alt interaction");
                OnDoubleTapAltInteract?.Invoke();
            }
        }

        private void InteractOnCanceled(InputAction.CallbackContext ctx)
        {
            if (ctx.interaction is HoldInteraction)
            {
                Debug.Log("interaction Canceled");
                OnReleaseInteract?.Invoke();
            }
        }

        private void InteractOnPerformed(InputAction.CallbackContext ctx)
        {
            if (ctx.interaction is HoldInteraction)
            {
                Debug.Log("Holding interaction");
            }
            else if (ctx.interaction is MultiTapInteraction)
            {
                Debug.Log("Multi-taping interaction");
            }
        }

        private void MoveOnCanceled(InputAction.CallbackContext ctx)
        {
            OnDPadInput?.Invoke(ctx.ReadValue<Vector2>());
        }

        private void MoveOnPerformed(InputAction.CallbackContext ctx)
        {
            OnDPadInput?.Invoke(ctx.ReadValue<Vector2>());
        }
    }
}
