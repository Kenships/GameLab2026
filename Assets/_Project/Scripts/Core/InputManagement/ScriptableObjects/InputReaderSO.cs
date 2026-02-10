using _Project.Scripts.Core.InputManagement.Interfaces;
using _Project.Scripts.Util.CustomAttributes;
using Sisus.Init;
using UnityEngine;

namespace _Project.Scripts.Core.InputManagement.ScriptableObjects
{
    [Service(typeof(IPlayerReader), typeof(IUIReader), typeof(IOverrideReader), typeof(IInputActionSetter), ResourcePath = "ScriptableObjects/Input/InputReaderSO")]
    [CreateAssetMenu(fileName = "InputReaderSO", menuName = "Scriptable Objects/Input/InputReaderSO")]
    public partial class InputReaderSO  : ScriptableObject, IInputActionSetter
    {
        private InputSystemActions _inputSystemActions;
        [SerializeField, ReadOnly] ActionMap activeActionMap;
        
        private void OnEnable()
        {
            _inputSystemActions ??= new InputSystemActions();
            _inputSystemActions.UI.SetCallbacks(this);
            _inputSystemActions.Player.SetCallbacks(this);
            _inputSystemActions.Override.SetCallbacks(this);
            _inputSystemActions.Enable();
            
            SetAction(ActionMap.Default);
        }

        public void SetAction(ActionMap actionMap)
        {
            activeActionMap = actionMap;
            switch (actionMap)
            {
                case ActionMap.Player:
                    _inputSystemActions.Player.Enable();
                    _inputSystemActions.UI.Disable();
                    break;
                case ActionMap.UI:
                    _inputSystemActions.UI.Enable();
                    _inputSystemActions.Player.Disable();
                    break;
                case ActionMap.Disabled:
                    _inputSystemActions.UI.Disable();
                    _inputSystemActions.Player.Disable();
                    break;
                default:
                    _inputSystemActions.UI.Enable();
                    _inputSystemActions.Player.Enable();
                    break;
            }
        }
        
        private void OnDisable()
        {
            _inputSystemActions.Disable();
        }
    }
}
