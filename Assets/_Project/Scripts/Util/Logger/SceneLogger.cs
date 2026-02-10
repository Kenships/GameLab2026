using _Project.Scripts.Util.Logger.Interface;
using Sisus.Init;
using UnityEngine;
using ILogger = _Project.Scripts.Util.Logger.Interface.ILogger;

namespace _Project.Scripts.Util.Logger
{
    public class SceneLogger : MonoBehaviour<ILoggerFactory>, ILogger
    {
        [SerializeField] private bool isEnabled = true;
        private ILogger _logger;

        private void OnValidate()
        {
            #if UNITY_EDITOR
            SetActive(isEnabled);
            #endif
        }

        protected override void Init(ILoggerFactory playerReader)
        {
            _logger = playerReader.CreateLogger();
        }
        
        public void Log(string message)
        {
            _logger.Log(message);
        }

        public void LogWarning(string message)
        {
            _logger.LogWarning(message);
        }

        public void LogError(string message)
        {
            _logger.LogError(message);
        }

        public void SetActive(bool active)
        {
            if (_logger == null)
                return;
            
            _logger.SetActive(active);
        }
    }
}
