using UnityEngine.Events;

namespace _Project.Scripts.Core.InputManagement.Interfaces
{
    public interface IOverrideReader : IActionMapReader
    {
        public event UnityAction OnEscapeEvent;
    }
}
