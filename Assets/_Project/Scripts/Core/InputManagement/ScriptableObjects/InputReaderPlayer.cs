using _Project.Scripts.Core.InputManagement.Interfaces;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace _Project.Scripts.Core.InputManagement.ScriptableObjects
{
    public partial class InputReaderSO : InputSystemActions.IPlayerActions, IPlayerReader
    {
        public event UnityAction<bool> OnPlayerEnableEvent;
        
        public event UnityAction<Vector2> OnMoveEvent;
        public event UnityAction<Vector2> OnLookEvent;
        public event UnityAction<bool> OnAttackEvent;
        public event UnityAction OnInteractEvent;
        public event UnityAction OnCrouchEvent;
        public event UnityAction OnJumpEvent;
        public event UnityAction OnPreviousEvent;
        public event UnityAction OnNextEvent;
        public event UnityAction<bool> OnSprintEvent;
        public event UnityAction<Vector2> OnPlayerPointEvent;
        public void OnMove(InputAction.CallbackContext context)
        {
            if (context.performed || context.canceled)
            {
                OnMoveEvent?.Invoke(context.ReadValue<Vector2>());
            }
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            if (context.performed || context.canceled)
            {
                OnLookEvent?.Invoke(context.ReadValue<Vector2>());
            }
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnAttackEvent?.Invoke(true);
            }
            else if (context.canceled)
            {
                OnAttackEvent?.Invoke(false);
            }
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnInteractEvent?.Invoke();
            }
        }

        public void OnCrouch(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnCrouchEvent?.Invoke();
            }
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnJumpEvent?.Invoke();
            }
        }

        public void OnPrevious(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnPreviousEvent?.Invoke();
            }
        }

        public void OnNext(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnNextEvent?.Invoke();
            }
        }

        public void OnSprint(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnSprintEvent?.Invoke(true);
            }
            else if (context.canceled)
            {
                OnSprintEvent?.Invoke(false);
            }
        }

        public void OnPlayerPoint(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnPlayerPointEvent?.Invoke(context.ReadValue<Vector2>());
            }
        }
    }
}
