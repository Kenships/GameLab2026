using _Project.Scripts.Core.InputManagement.Interfaces;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace _Project.Scripts.Core.InputManagement.ScriptableObjects
{
    
    public partial class InputReaderSO : InputSystemActions.IOverrideActions, IOverrideReader
    {
        public event UnityAction OnEscapeEvent;
        
        public void OnEscape(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnEscapeEvent?.Invoke();
            }
        }
    }
}
