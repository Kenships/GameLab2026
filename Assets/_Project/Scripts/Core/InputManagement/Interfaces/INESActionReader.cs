using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace _Project.Scripts.Core.InputManagement.Interfaces
{
    public interface INESActionReader
    {
        event UnityAction<Vector2> OnDPadInput;
        event UnityAction OnHoldInteract;
        event UnityAction OnReleaseInteract;
        event UnityAction OnTapInteract;
        event UnityAction OnHoldAltInteract;
        event UnityAction OnReleaseAltInteract;
        event UnityAction OnTapAltInteract;

        event UnityAction OnEscape;

        public bool TryGetGamePad(out Gamepad gamePad);
    }
}
