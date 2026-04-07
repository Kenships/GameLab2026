using System;
using System.Collections.Generic;
using _Project.Scripts.Core.InputManagement;
using _Project.Scripts.Effects;
using _Project.Scripts.UI;
using _Project.Scripts.Util.CustomAttributes;
using Knot.Localization;
using Knot.Localization.Data;
using UnityEngine;
using UnityEngine.Events;

namespace _Project.Scripts.Tutorial
{
    public class TutorialComposer : MonoBehaviour
    {
        [Serializable]
        private struct PanelEvent
        {
            [SerializeField] public KnotTextKeyReference PanelTextKey;
            public bool FreezePlayers;
            public UnityEvent<Action> OnPanel;
        }
        
        [SerializeField] private List<PanelEvent> panels;
        [SerializeField] private TextTyper textTyper;
        [SerializeField] private GameObject panel;
        [SerializeField] private NESActionReader player1Actions;
        [SerializeField] private NESActionReader player2Actions;
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
            if (panel == null) return;
            
            panel.SetActive(true);
            panels[_panelIndex].OnPanel?.Invoke(Next);
            textTyper.StartTyping(panels[_panelIndex].PanelTextKey);
            
            if (panels[_panelIndex].FreezePlayers)
            {
                player1Actions.enabled = false;
                player2Actions.enabled = false;
            }
            else
            {
                player1Actions.enabled = true;
                player2Actions.enabled = true;
            }
        }

        private enum Position
        {
            Top,
            Bottom
        }
    }
}
