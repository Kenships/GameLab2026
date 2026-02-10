using Sisus.Init;
using UnityEngine;
using ILogger = _Project.Scripts.Util.Logger.Interface.ILogger;

namespace _Project.Scripts.Util.Logger
{
    
    public class Logger : ILogger
    {
        private bool _enabled = true;
        public void Log(string message)
        {
            if (!_enabled)
                return;
            Debug.Log(message);
        }

        public void LogWarning(string message)
        {
            if (!_enabled)
                return;
            Debug.LogWarning(message);
        }

        public void LogError(string message)
        {
            if (!_enabled)
                return;
            Debug.LogError(message);
        }

        public void SetActive(bool active)
        {
            _enabled = active;
        }
    }
}
