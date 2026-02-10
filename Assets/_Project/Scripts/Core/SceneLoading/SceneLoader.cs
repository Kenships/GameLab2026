using System.Collections.Generic;
using _Project.Scripts.Core.InputManagement;
using _Project.Scripts.Core.InputManagement.Interfaces;
using _Project.Scripts.Core.SceneLoading.Interfaces;
using _Project.Scripts.Util.CustomAttributes;
using _Project.Scripts.Util.Scene;
using Sisus.Init;
using UnityEngine;
using ILogger = _Project.Scripts.Util.Logger.Interface.ILogger;

namespace _Project.Scripts.Core.SceneLoading
{
    public class SceneLoader : MonoBehaviour<ISceneBuilder,IInputActionSetter, ILogger>
    {
        [SerializeField] private bool loadOnAwake;
        [SerializeField] private bool setActive;
        [SerializeField, ShowIf(nameof(setActive))] private SceneReference activeSceneIndex;
        [SerializeField] private List<SceneReference> scenesToLoad;
        [SerializeField] private bool unloadDisabled;
        [SerializeField] private List<SceneReference> scenesToUnload;
        [SerializeField] private bool withOverlay;
        [SerializeField] private SceneController.SceneGroup sceneGroup = SceneController.SceneGroup.None;
        [SerializeField] private ActionMap actionMap = ActionMap.Default;
        
        private ISceneBuilder _sceneController;
        private IInputActionSetter _inputActionSetter;
        private ILogger _logger;
        
        protected override void Init(ISceneBuilder sceneBuilder, IInputActionSetter inputActionSetter, ILogger logger)
        {
            _sceneController = sceneBuilder;
            _inputActionSetter = inputActionSetter;
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
                loadingStrategy.Unload(scene.BuildIndex);
            }
            
            loadingStrategy
                .WithOverlay(withOverlay)
                .Execute();
        }
    }
}
