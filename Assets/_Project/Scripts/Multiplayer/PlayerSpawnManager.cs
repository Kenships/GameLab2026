using _Project.Scripts.Core.InputManagement;
using _Project.Scripts.Core.Player;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Project.Scripts.Multiplayer
{
    public class PlayerSpawnManager : MonoBehaviour
    {
        [SerializeField] private Transform[] spawnPoints;

        private int _playerCount = 0;

        public void OnPlayerJoined(PlayerInput playerInput)
        {
            if (!playerInput)
            {
                Debug.LogError("PlayerInput is null");
                return;
            }
            
            var actions = new NESActions();
            actions.asset.devices = playerInput.devices;
            actions.Enable();

            playerInput.GetOrAddComponent<NESActionReader>().Init(actions);

            foreach (Transform spawnPoint in spawnPoints)
            {
                Debug.Log(spawnPoint.name + " " + spawnPoint.position);
            }
            
            playerInput.transform.position = spawnPoints[_playerCount].position;
            
            _playerCount++;
        }
    }
}
