using _Project.Scripts.UI.UIElements;
using Obvious.Soap;
using UnityEngine;

namespace _Project.Scripts.UI.UIBindings
{
    [AddComponentMenu("Soap/Bindings/BindSlideToggle")]
    [RequireComponent(typeof(SlideToggle))]
    public class BindSlideToggle : CacheComponent<SlideToggle>
    {
        [SerializeField] private BoolVariable boolVariable = null;

        protected override void Awake()
        {
            base.Awake();
            OnValueChanged(boolVariable);
            _component.onValueChanged.AddListener(SetBoundVariable);
            boolVariable.OnValueChanged += OnValueChanged;
        }

        private void OnDestroy()
        {
            _component.onValueChanged.RemoveListener(SetBoundVariable);
            boolVariable.OnValueChanged -= OnValueChanged;
        }

        private void OnValueChanged(bool value)
        {
            _component.IsOn = value;
        }

        private void SetBoundVariable(bool value)
        {
            boolVariable.Value = value;
        }
    }
}
