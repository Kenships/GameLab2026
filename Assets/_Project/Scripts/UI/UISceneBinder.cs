using System;
using _Project.Scripts.Core.SceneLoading.Interfaces;
using Sisus.Init;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.UI
{
    [RequireComponent(typeof(Selectable))]
    public class UISceneBinder : MonoBehaviour<ISceneFocusRetrieval>
    {
        private ISceneFocusRetrieval _sceneFocus;
        
        protected override void Init(ISceneFocusRetrieval sceneFocus)
        {
            _sceneFocus = sceneFocus;
        }
        
        private int _sceneBuildIndex;
        private Selectable _selectable;
        
        protected override void OnAwake()
        {
            _sceneBuildIndex = gameObject.scene.buildIndex;
            _selectable = GetComponent<Selectable>();
        }

        private void Update()
        {
            _selectable.interactable = _sceneFocus.IsFocused(_sceneBuildIndex);
        }

        
    }
}
