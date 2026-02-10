namespace _Project.Scripts.Util.Logger.Interface
{
    public interface ILogger
    {
        void Log(string message);
        void LogWarning(string message);
        void LogError(string message);
        void SetActive(bool active);
    }
}
