using System.Collections.Generic;
using _Project.Scripts.Core.SceneLoading.Interfaces;
using _Project.Scripts.Util.Scene;
using Sisus.Init;
using UnityEngine;
using ILogger = _Project.Scripts.Util.Logger.Interface.ILogger;

namespace _Project.Scripts.Core.SceneLoading
{
    public class SceneUnloader : MonoBehaviour<ISceneBuilder, ILogger>
    {
        [SerializeField] private List<SceneReference> scenesToUnload;
        [SerializeField] private bool withOverlay;
        [SerializeField] private bool disable;
        
        private ISceneBuilder _sceneController;
        private ILogger _logger;
        protected override void Init(ISceneBuilder sceneBuilder, ILogger logger)
        {
            _sceneController = sceneBuilder;
            _logger = logger;
        }

        public void UnloadScene()
        {
            
            SceneController.SceneLoadingStrategy loadingStrategy = _sceneController.NewStrategy();
            
            foreach (var sceneRef in scenesToUnload)
            {
                if (sceneRef.BuildIndex == 0)
                {
                    _logger.LogError($"GameObject: {gameObject.name} from Scene: {gameObject.scene.name} " +
                                     $"Tried to unload BootStrap. Skip Scene unloading");
                    return;
                }

                if (disable)
                {
                    loadingStrategy.Disable(sceneRef.BuildIndex);
                }
                else
                {
                    loadingStrategy.Unload(sceneRef.BuildIndex);
                }
            }
            
            loadingStrategy.WithOverlay(withOverlay).Execute();
        }
    }
}
