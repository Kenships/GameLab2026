using UnityEngine;

namespace _Project.Scripts.Core.Player
{
    public class PlayerData : MonoBehaviour
    {
        public enum PlayerID
        {
            Player1,
            Player2,
        }
        
        
        [SerializeField] private PlayerID playerID;
        public PlayerID ID => playerID;
    }
}
