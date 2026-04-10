using _Project.Scripts.Multiplayer;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace _Project.Scripts.Core.InputManagement
{
    public abstract class NESActionBase : INESAction
    {
        public event UnityAction<InputAction.CallbackContext> MovePerformed;
        public event UnityAction<InputAction.CallbackContext> MoveCancelled;
        public event UnityAction<InputAction.CallbackContext> InteractPerformed;
        public event UnityAction<InputAction.CallbackContext> InteractCancelled;
        public event UnityAction<InputAction.CallbackContext> AltInteractPerformed;
        public event UnityAction<InputAction.CallbackContext> AltInteractCancelled;
        
        public event UnityAction<InputAction.CallbackContext> UINavigatePerformed;
        public event UnityAction<InputAction.CallbackContext> UISubmitPerformed;
        public event UnityAction<InputAction.CallbackContext> UICancelPerformed;
        
        public event UnityAction<InputAction.CallbackContext> OverridePerformed;
        
        private NESActions _actions;

        protected NESActionBase(NESActions action)
        {
            _actions = action;
            _actions.UI.Navigate.performed += (ctx) => UINavigatePerformed?.Invoke(ctx);
            _actions.UI.Submit.performed += (ctx) => UISubmitPerformed?.Invoke(ctx);
            _actions.UI.Cancel.performed += (ctx) => UICancelPerformed?.Invoke(ctx);
            _actions.Override.Escape.performed += (ctx) => OverridePerformed?.Invoke(ctx);
        }
        
        protected void OnMovePerformed(InputAction.CallbackContext ctx) => MovePerformed?.Invoke(ctx);
        protected void OnMoveCancelled(InputAction.CallbackContext ctx) => MoveCancelled?.Invoke(ctx);
        protected void OnInteractPerformed(InputAction.CallbackContext ctx) => InteractPerformed?.Invoke(ctx);
        protected void OnInteractCancelled(InputAction.CallbackContext ctx) => InteractCancelled?.Invoke(ctx);
        protected void OnAltInteractPerformed(InputAction.CallbackContext ctx) => AltInteractPerformed?.Invoke(ctx);
        protected void OnAltInteractCancelled(InputAction.CallbackContext ctx) => AltInteractCancelled?.Invoke(ctx);
        
        public ReadOnlyArray<InputDevice>? devices => _actions.devices;

        public void Disable()
        {
            _actions.Disable();
        }

        public void Enable()
        {
            _actions.Enable();
        }
    }
}
