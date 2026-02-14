using System;
using _Project.Scripts.Core.InputManagement;
using _Project.Scripts.Core.Player;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace _Project.Scripts.Multiplayer
{
    [RequireComponent(typeof(PlayerInputManager))]
    public class PlayerSpawnManager : MonoBehaviour
    {
        [SerializeField] private Transform[] spawnPoints;
        private PlayerInputManager _playerInputManager;
        private int _playerCount = 0;

        private void Awake()
        {
            _playerInputManager = GetComponent<PlayerInputManager>();
        }

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

            playerInput.actions = actions.asset;


            playerInput.GetOrAddComponent<NESActionReader>().Init(actions);
            
            playerInput.transform.position = spawnPoints[_playerCount].position;
            
            _playerCount++;
        }
    }
}
