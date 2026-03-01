using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace _Project.Scripts.Core.InputManagement.Interfaces
{
    public interface INESActionReader
    {
        event UnityAction<Vector2> OnDPadInput;
        event UnityAction OnTapInteract;
        event UnityAction OnHoldInteract;
        event UnityAction OnReleaseInteract;
        event UnityAction OnDoubleTapInteract;
        event UnityAction OnTapAltInteract;
        event UnityAction OnHoldAltInteract;
        event UnityAction OnReleaseAltInteract;
        event UnityAction OnDoubleTapAltInteract;

        public bool TryGetGamePad(out Gamepad gamePad);
    }
}
