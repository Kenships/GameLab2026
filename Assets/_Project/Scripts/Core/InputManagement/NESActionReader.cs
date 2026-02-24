using System;
using _Project.Scripts.Core.InputManagement.Interfaces;
using Sisus.Init;
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
        public event UnityAction OnTapInteract;
        public event UnityAction OnHoldInteract;
        public event UnityAction OnReleaseInteract;
        public event UnityAction OnDoubleTapInteract;
        public event UnityAction OnTapAltInteract;
        public event UnityAction OnHoldAltInteract;
        public event UnityAction OnReleaseAltInteract;
        public event UnityAction OnDoubleTapAltInteract;

        private PlayerInput _playerInput;
        private NESActions _actions;

        private InputAction _dpad;
        private InputAction _interact;
        private InputAction _altInteract;
        
        public void Init(NESActions actions)
        {
            _actions = actions;
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
                //Debug.Log("AltInteract Canceled");
                OnReleaseAltInteract?.Invoke();
            }
        }

        private void AltInteractOnPerformed(InputAction.CallbackContext ctx)
        {
            if (ctx.interaction is TapInteraction)
            {
                OnTapAltInteract?.Invoke();
            }
            else if (ctx.interaction is HoldInteraction)
            {
                OnHoldAltInteract?.Invoke();
            }
            else if (ctx.interaction is MultiTapInteraction)
            {
                OnDoubleTapAltInteract?.Invoke();
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
            if (ctx.interaction is TapInteraction)
            {
                //Debug.Log("Interact On Tap");
                OnTapInteract?.Invoke();
            }
            else if (ctx.interaction is HoldInteraction)
            {
                //Debug.Log("Holding interaction");
                OnHoldInteract?.Invoke();
            }
            else if (ctx.interaction is MultiTapInteraction)
            {
                //Debug.Log("Multi-taping interaction");
                OnDoubleTapInteract?.Invoke();
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
