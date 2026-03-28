using System;
using System.Collections.Generic;
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
        
        private void Start()
        {
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
            panels[_panelIndex].OnPanel?.Invoke(Next);
            textTyper.StartTyping(panels[_panelIndex].PanelText);
        }
    }
}
