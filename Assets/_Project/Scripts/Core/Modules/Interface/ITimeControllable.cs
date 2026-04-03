using _Project.Scripts.Core.Player;

namespace _Project.Scripts.Core.Modules.Interface
{
    public interface ITimeControllable
    {
        void Rewind(PlayerData.PlayerID playerID);
        void CancelRewind(PlayerData.PlayerID playerID);
        void FastForward(PlayerData.PlayerID playerID);
        void CancelFastForward(PlayerData.PlayerID playerID);
    }
}
