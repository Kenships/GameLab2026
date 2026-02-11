using UnityEngine;
using UnityEngine.Events;

namespace _Project.Scripts.Core.InputManagement.Interfaces
{
    public class INESActionReader
    {
        public UnityEvent<Vector2> OnDPadInput;
        public UnityEvent OnHoldInteract;
        public UnityEvent OnReleaseInteract;
        public UnityEvent OnDoubleTapInteract;
        public UnityEvent OnHoldAltInteract;
        public UnityEvent OnReleaseAltInteract;
        public UnityEvent OnDoubleTapAltInteract;
    }
}
