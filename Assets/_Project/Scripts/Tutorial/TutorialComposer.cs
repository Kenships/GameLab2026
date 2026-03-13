using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace _Project.Scripts.Tutorial
{
    public class TutorialComposer : MonoBehaviour
    {
        [Serializable]
        private struct PanelEvent
        {
            public GameObject Panel;
            public UnityEvent OnPanel;
        }
        
        [SerializeField] private List<PanelEvent> panels;

        private int _panelIndex;
        private GameObject _currentPanel;

        private void Start()
        {
            _currentPanel = panels[_panelIndex].Panel;
            _currentPanel.SetActive(true);
            panels[_panelIndex].OnPanel?.Invoke();
            foreach (PanelEvent panel in panels)
            {
                if (panel.Panel != _currentPanel)
                {
                    panel.Panel.SetActive(false);
                }
            }
        }

        public void Next()
        {
            _currentPanel.SetActive(false);
            _panelIndex = Mathf.Clamp(_panelIndex + 1, 0, panels.Count - 1);
            _currentPanel = panels[_panelIndex].Panel;
            panels[_panelIndex].OnPanel?.Invoke();
            _currentPanel.SetActive(true);
        }
    }
}
