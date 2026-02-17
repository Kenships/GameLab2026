using UnityEngine;
using UnityEngine.Events;

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
    }
}
