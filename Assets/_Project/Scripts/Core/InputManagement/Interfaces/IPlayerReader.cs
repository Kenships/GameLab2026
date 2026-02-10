using UnityEngine;
using UnityEngine.Events;

namespace _Project.Scripts.Core.InputManagement.Interfaces
{
    public interface IPlayerReader : IActionMapReader
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
    }
}
