using _Project.Scripts.Core.Player;

namespace _Project.Scripts.Core.Modules.Interface
{
    public interface IVisualSelectable
    {
        void ShowVisual(PlayerData.PlayerID playerID);
        void HideVisual(PlayerData.PlayerID playerID);
    }
}
