using _Project.Scripts.Core.InputManagement.Interfaces;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace _Project.Scripts.Core.InputManagement.ScriptableObjects
{
    
    public partial class InputReaderSO : InputSystemActions.IUIActions, IUIReader
    {
        public event UnityAction<bool> OnUIEnableEvent;
        
        public event UnityAction<Vector2> OnNavigateEvent;
        public event UnityAction OnSubmitEvent;
        public event UnityAction OnCancelEvent;
        public event UnityAction<Vector2> OnPointEvent;
        public event UnityAction OnClickEvent;
        public event UnityAction OnRightClickEvent;
        public event UnityAction OnMiddleClickEvent;
        public event UnityAction<Vector2> OnScrollWheelEvent;
        public event UnityAction<Vector3> OnTrackedDevicePositionEvent;
        public event UnityAction<Quaternion> OnTrackedDeviceOrientationEvent;

        public void OnNavigate(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnNavigateEvent?.Invoke(context.ReadValue<Vector2>());
            }
        }

        public void OnSubmit(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnSubmitEvent?.Invoke();
            }
        }

        public void OnCancel(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnCancelEvent?.Invoke();
            }
        }

        public void OnPoint(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnPointEvent?.Invoke(context.ReadValue<Vector2>());
            }
        }

        public void OnClick(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnClickEvent?.Invoke();
            }
        }

        public void OnRightClick(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnRightClickEvent?.Invoke();
            }
        }

        public void OnMiddleClick(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnMiddleClickEvent?.Invoke();
            }
        }

        public void OnScrollWheel(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnScrollWheelEvent?.Invoke(context.ReadValue<Vector2>());
            }
        }

        public void OnTrackedDevicePosition(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnTrackedDevicePositionEvent?.Invoke(context.ReadValue<Vector3>());
            }
        }

        public void OnTrackedDeviceOrientation(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnTrackedDeviceOrientationEvent?.Invoke(context.ReadValue<Quaternion>());
            }
        }
    }
}
