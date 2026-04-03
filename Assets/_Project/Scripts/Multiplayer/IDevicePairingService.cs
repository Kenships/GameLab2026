using UnityEngine;
using UnityEngine.Events;

namespace _Project.Scripts.Multiplayer
{
    public interface IDevicePairingService
    {
        public event UnityAction OnPlayer1Paired;
        public event UnityAction OnPlayer2Paired;
        public event UnityAction OnBothPlayersPaired;
        public event UnityAction OnPlayer1Unpaired;
        public event UnityAction OnPlayer2Unpaired;
        
        bool IsPlayer1Paired { get; }
        bool IsPlayer2Paired { get; }
        
        bool SwapPlayers { get; set; }

        bool TryGetFor(Component client, out NESActions value);
    }
}
