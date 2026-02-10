using UnityEngine.Events;

namespace _Project.Scripts.Core.SceneLoading.Interfaces
{
    public interface ISceneBuilder
    {
        public UnityAction<int> OnSceneLoaded { get; set; }
        public UnityAction OnBeforeSceneLoad { get; set; }
        public UnityAction OnLoadRoutineComplete { get; set; }
        SceneController.SceneLoadingStrategy NewStrategy();
    }
}
