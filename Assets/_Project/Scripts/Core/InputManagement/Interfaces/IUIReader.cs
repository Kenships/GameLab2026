using UnityEngine;
using UnityEngine.Events;

namespace _Project.Scripts.Core.InputManagement.Interfaces
{
    public interface IUIReader : IActionMapReader
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
    }
}
