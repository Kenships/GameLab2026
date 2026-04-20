using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace _Project.Scripts.Multiplayer
{
    public interface INESAction
    {
        event UnityAction<InputAction.CallbackContext> MovePerformed;
        event UnityAction<InputAction.CallbackContext> MoveCancelled;
        event UnityAction<InputAction.CallbackContext> InteractPerformed;
        event UnityAction<InputAction.CallbackContext> InteractCancelled;
        event UnityAction<InputAction.CallbackContext> AltInteractPerformed;
        event UnityAction<InputAction.CallbackContext> AltInteractCancelled;
        
        event UnityAction<InputAction.CallbackContext> UINavigatePerformed;
        event UnityAction<InputAction.CallbackContext> UISubmitPerformed;
        event UnityAction<InputAction.CallbackContext> UICancelPerformed;
        
        event UnityAction<InputAction.CallbackContext> OverridePerformed;

        ReadOnlyArray<InputDevice>? devices { get; }
        
        void Disable();
        void Enable();
    }
}
