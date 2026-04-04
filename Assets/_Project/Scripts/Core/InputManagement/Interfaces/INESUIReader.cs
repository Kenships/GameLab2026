using UnityEngine;
using UnityEngine.Events;

namespace _Project.Scripts.Core.InputManagement.Interfaces
{
    public interface INESUIReader
    {
        event UnityAction<Vector2> OnNavigate;
        event UnityAction OnSubmit;
        event UnityAction OnCancel;
        event UnityAction OnEscape;
    }
}
