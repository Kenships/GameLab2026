using System.Collections.Generic;

namespace _Project.Scripts.Core.SceneLoading.Interfaces
{
    public interface ISceneFocusRetrieval
    {
        bool IsFocused(int buildIndex);
        List<int> GetFocusedScenes();
    }
}
