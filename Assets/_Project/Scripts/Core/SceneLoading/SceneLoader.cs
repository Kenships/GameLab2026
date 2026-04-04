using System.Collections.Generic;
using _Project.Scripts.Core.AudioPooling.Interface;
using _Project.Scripts.Core.InputManagement;
using _Project.Scripts.Core.InputManagement.Interfaces;
using _Project.Scripts.Core.SceneLoading.Interfaces;
using _Project.Scripts.Util.CustomAttributes;
using _Project.Scripts.Util.Scene;
using Sisus.Init;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using ILogger = _Project.Scripts.Util.Logger.Interface.ILogger;

namespace _Project.Scripts.Core.SceneLoading
{
    public class SceneLoader : MonoBehaviour<ISceneBuilder, IInputActionSetter, IAudioPoolSceneController, ILogger>
    {
        public UnityEvent onBeforeSceneLoad;

        [SerializeField] private bool loadOnAwake;
        [SerializeField] private bool setActive;

        [SerializeField, ShowIf(nameof(setActive))]
        private SceneReference activeSceneIndex;

        [SerializeField] private bool withOverlay;
        [SerializeField] private bool fadeAudio = true;

        [SerializeField, ShowIf(nameof(fadeAudio))]
        private float fadeDuration = 1f;

        [SerializeField] private SceneController.SceneGroup sceneGroup = SceneController.SceneGroup.None;
        [SerializeField] private ActionMap actionMap = ActionMap.Default;

        [SerializeField] private List<SceneReference> scenesToLoad;
        [SerializeField] private bool unloadDisabled;
        [SerializeField] private List<SceneReference> scenesToUnload;

        private ISceneBuilder _sceneController;
        private IInputActionSetter _inputActionSetter;
        private IAudioPoolSceneController _audioController;
        private ILogger _logger;

        protected override void Init(ISceneBuilder sceneBuilder, IInputActionSetter inputActionSetter,
            IAudioPoolSceneController audioController, ILogger logger)
        {
            _sceneController = sceneBuilder;
            _inputActionSetter = inputActionSetter;
            _audioController = audioController;
            _logger = logger;
        }

        protected override void OnAwake()
        {
            if (loadOnAwake)
            {
                LoadScene();
            }
        }

        public void LoadScene()
        {
            onBeforeSceneLoad?.Invoke();
            SceneController.SceneLoadingStrategy loadingStrategy =
                _sceneController
                    .NewStrategy()
                    .SetSceneGroup(sceneGroup);

            _inputActionSetter.SetAction(actionMap);

            foreach (var scene in scenesToLoad)
            {
                if (scene.BuildIndex == 0)
                {
                    _logger.LogError($"GameObject: {gameObject.name} from Scene: {gameObject.scene.name} " +
                                     $"Tried to load BootStrap. Skip Scene loading");
                    return;
                }

                loadingStrategy.Load(scene.BuildIndex, setActive && scene.BuildIndex == activeSceneIndex.BuildIndex);
            }

            foreach (var scene in scenesToUnload)
            {
                if (scene.BuildIndex == 0)
                {
                    _logger.LogError($"GameObject: {gameObject.name} from Scene: {gameObject.scene.name} " +
                                     $"Tried to unload BootStrap. Skip Scene unloading");
                    continue;
                }

                if (unloadDisabled)
                {
                    loadingStrategy.Disable(scene.BuildIndex);
                }
                else
                {
                    loadingStrategy.Unload(scene.BuildIndex);
                }

                if (fadeAudio)
                {
                    _audioController.FadeAllVolumeFromScene(scene.BuildIndex, 0f, fadeDuration);
                }
            }

            loadingStrategy
                .WithOverlay(withOverlay)
                .Execute();
        }
    }
}
