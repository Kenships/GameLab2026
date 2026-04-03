using System;
using System.Collections.Generic;
using _Project.Scripts.Effects;
using _Project.Scripts.UI;
using _Project.Scripts.Util.CustomAttributes;
using Obvious.Soap;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace _Project.Scripts.Tutorial
{
    public class TutorialComposer : MonoBehaviour
    {
        [Serializable]
        private struct PanelEvent
        {
            [DynamicTextArea] public string PanelText;
            public UnityEvent<Action> OnPanel;
        }
        
        [SerializeField] private List<PanelEvent> panels;
        [SerializeField] private TextTyper textTyper;
        [SerializeField] private GameObject panel;
        private int _panelIndex;

        private float _posOffset;
        
        private void Start()
        {
            var rt = (RectTransform)panel.transform;
            _posOffset = rt.anchoredPosition.y;
            
            RefreshPanel();
        }

        public void Next()
        {
            _panelIndex = Mathf.Clamp(_panelIndex + 1, 0, panels.Count - 1);
            RefreshPanel();
        }

        private void RefreshPanel()
        {
            panel.SetActive(true);
            AnchorBottom();
            panels[_panelIndex].OnPanel?.Invoke(Next);
            textTyper.StartTyping(panels[_panelIndex].PanelText);
        }

        private enum Position
        {
            Top,
            Bottom
        }

        public void AnchorTop()
        {
            Anchor(Position.Top);
        }

        public void AnchorBottom()
        {
            Anchor(Position.Bottom);
        }

        private void Anchor(Position position)
        {
            var rt = (RectTransform)panel.transform;
            switch (position)
            {
                case Position.Top:
                    rt.anchorMin = new Vector2(0.5f, 1f);
                    rt.anchorMax = new Vector2(0.5f, 1f);
                    rt.pivot     = new Vector2(0.5f, 1f);
                    rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, -_posOffset);
                    break;
                case Position.Bottom:
                    rt.anchorMin = new Vector2(0.5f, 0f);
                    rt.anchorMax = new Vector2(0.5f, 0f);
                    rt.pivot     = new Vector2(0.5f, 0f);
                    rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, _posOffset);
                    break;
            }
        }
    }
}
