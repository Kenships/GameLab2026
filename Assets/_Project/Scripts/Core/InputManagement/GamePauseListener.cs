using System;
using System.Collections;
using _Project.Scripts.Core.AudioPooling;
using _Project.Scripts.Core.SceneLoading.Interfaces;
using _Project.Scripts.Util.Scene;
using Sisus.Init;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Project.Scripts.Core.InputManagement
{
    public class GamePauseListener : MonoBehaviour<ISceneBuilder, ISceneFocusRetrieval, AudioPooler>
    {
        [SerializeField] private SceneReference menuSelect;
        [SerializeField] private SceneReference pauseScene;
        private NESActionReader[] _readers;
        private ISceneBuilder _sceneBuilder;
        private ISceneFocusRetrieval _sceneFocusRetrieval;
        private AudioPooler _audioPooler;

        private bool _isPaused;
        private float _previousTimeScale;
        
        private void Start()
        {
            _readers = FindObjectsByType<NESActionReader>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            foreach (NESActionReader reader in _readers)
            {
                reader.OnEscape += PlayerOnEscape;
            }
        }

        private void OnDestroy()
        {
            foreach (NESActionReader reader in _readers)
            {
                reader.OnEscape -= PlayerOnEscape;
            }
        }

        private void PlayerOnEscape()
        {
            if (_isPaused)
                return;
            _audioPooler.StopAllSFX();
            _previousTimeScale = Time.timeScale;
            Time.timeScale = 0;
            StartCoroutine(LoadScene());
        }

        private IEnumerator LoadScene()
        {
            yield return _sceneBuilder
                .NewStrategy()
                .Load(menuSelect.BuildIndex)
                .Load(pauseScene.BuildIndex)
                .Execute();
            
            _isPaused = true;
        }

        private void Update()
        {
            if (!_isPaused) return;

            if (_sceneFocusRetrieval.IsFocused(gameObject.scene.buildIndex))
            {
                Time.timeScale = _previousTimeScale;
                _isPaused = false;
            }
        }

        protected override void Init(ISceneBuilder argument, ISceneFocusRetrieval retrieval, AudioPooler audioPooler)
        {
            _sceneBuilder = argument;
            _sceneFocusRetrieval = retrieval;
            _audioPooler = audioPooler;
        }
    }
}
