using System.Collections.Generic;
using _Project.Scripts.Core.SceneLoading.Interfaces;
using _Project.Scripts.Util.CustomAttributes;
using _Project.Scripts.Util.Scene;
using Sisus.Init;
using UnityEngine;

namespace _Project.Scripts.Core.SceneLoading
{
    public class SceneFocusDebugger : MonoBehaviour<ISceneFocusRetrieval>
    {
        [SerializeField, ReadOnly] private List<SceneReference> focusedScenes;
        ISceneFocusRetrieval _sceneFocusRetrieval;
        protected override void Init(ISceneFocusRetrieval playerReader)
        {
            _sceneFocusRetrieval = playerReader;
        }

        private void FixedUpdate()
        {
            focusedScenes.Clear();
            List<int> buildIndices = _sceneFocusRetrieval.GetFocusedScenes();

            foreach (int buildIndex in buildIndices)
            {
                focusedScenes.Add(new SceneReference()
                {
                    BuildIndex = buildIndex
                });
            }
        }
    }
}
