using System;
using _Project.Scripts.Core.Player;
using _Project.Scripts.Core.SceneLoading;
using UnityEngine;

namespace _Project.Scripts.UI.UIElements
{
    public class LevelSelectPanel : MonoBehaviour
    {
        [SerializeField] private GameObject player1Select;
        [SerializeField] private GameObject player2Select;
        [SerializeField] private GameObject player1Confirm;
        [SerializeField] private GameObject player2Confirm;

        private SceneLoader _sceneLoader;
        
        private void Awake()
        {
            _sceneLoader = GetComponent<SceneLoader>();
        }

        public void LoadLevel()
        {   
            _sceneLoader.LoadScene();
        }

        public void Deselect(PlayerData.PlayerID playerID)
        {
            switch (playerID)
            {
                case PlayerData.PlayerID.Player1:
                    player1Select.SetActive(false);
                    player1Confirm.SetActive(false);
                    break;
                case PlayerData.PlayerID.Player2:
                    player2Select.SetActive(false);
                    player2Confirm.SetActive(false);
                    break;
            }
        }
        
        public void Select(PlayerData.PlayerID playerID)
        {
            switch (playerID)
            {
                case PlayerData.PlayerID.Player1:
                    player1Select.SetActive(true);
                    player1Confirm.SetActive(false);
                    break;
                case PlayerData.PlayerID.Player2:
                    player2Select.SetActive(true);
                    player2Confirm.SetActive(false);
                    break;
            }
        }

        public void Confirm(PlayerData.PlayerID playerID)
        {
            switch (playerID)
            {
                case PlayerData.PlayerID.Player1:
                    player1Confirm.SetActive(true);
                    break;
                case PlayerData.PlayerID.Player2:
                    player2Confirm.SetActive(true);
                    break;
            }
        }
    }
}
